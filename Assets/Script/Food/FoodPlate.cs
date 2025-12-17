using UnityEngine;
using System.Collections.Generic;

public class FoodPlate : MonoBehaviour
{
    public PlateProgressBar progressBar;
    public List<FoodItem> foods = new();

    [Header("Spawn on take")]
    public GameObject emptyPlatePrefab;
    public Transform spawnPoint;

    [Header("Layout")]
    public int slotsPerRow = 3; // max items per row on the plate
    public float itemSpacingX = 0.12f;
    public float itemSpacingZ = 0.12f;
    public float itemYOffset = 0.02f; // slight y offset per row to prevent z-fighting

    public void AddFood(FoodItem food)
    {
        if (food == null) return;

        if (food.foodType == FoodType.Junk)
        {
            progressBar.AddJunk(25);
            Destroy(food.gameObject);
            return;
        }

        progressBar.AddHealthy(25);

        // preserve world scale before reparenting so we don't inadvertently stretch the model
        Vector3 worldScale = food.transform.lossyScale;

        foods.Add(food);

        // parent to plate and compute neat grid position (no stacking)
        food.transform.SetParent(transform);

        int index = foods.Count - 1;
        int row = index / slotsPerRow;
        int col = index % slotsPerRow;

        int rowsUsed = (foods.Count + slotsPerRow - 1) / slotsPerRow;
        float offsetX = ((slotsPerRow - 1) * itemSpacingX) / 2f;
        float offsetZ = ((rowsUsed - 1) * itemSpacingZ) / 2f;

        float x = col * itemSpacingX - offsetX;
        float z = -(row * itemSpacingZ) + offsetZ;
        float y = itemYOffset * row;

        food.transform.localPosition = new Vector3(x, y, z);
        food.transform.localRotation = Quaternion.identity;

        // preserve world scale relative to this plate's scale to avoid stretching
        Vector3 parentScale = transform.lossyScale;
        Vector3 localScale = new Vector3(
            parentScale.x != 0f ? worldScale.x / parentScale.x : worldScale.x,
            parentScale.y != 0f ? worldScale.y / parentScale.y : worldScale.y,
            parentScale.z != 0f ? worldScale.z / parentScale.z : worldScale.z);
        food.transform.localScale = localScale;

        // disable colliders so food placed on plate can't be targeted/picked up
        foreach (var c in food.GetComponentsInChildren<Collider>())
            c.enabled = false;

        // move food to Ignore Raycast layer (if available) so raycasts/spherecasts won't hit it; fallback no-op if layer missing
        int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
        if (ignoreLayer != -1)
        {
            foreach (Transform t in food.GetComponentsInChildren<Transform>())
                t.gameObject.layer = ignoreLayer;
        }

        if (food.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // apakah plate ini bisa diambil pemain (misal bar penuh)
    public bool CanBeTaken()
    {
        return progressBar != null && progressBar.IsFull();
    }

    // dipanggil saat pemain mengambil plate ini dari meja
    public void OnTakenByPlayer()
    {
        // spawn plate kosong pengganti di spawnPoint jika tersedia
        if (emptyPlatePrefab != null && spawnPoint != null)
        {
            GameObject newPlate = Instantiate(emptyPlatePrefab, spawnPoint.position, spawnPoint.rotation);
            var fp = newPlate.GetComponent<FoodPlate>();
            if (fp != null) fp.progressBar = this.progressBar;
            var pi = newPlate.GetComponent<PlateInteract>();
            if (pi != null) pi.progressBar = this.progressBar;
        }

        if (progressBar != null) progressBar.ResetBar();
    }
}
