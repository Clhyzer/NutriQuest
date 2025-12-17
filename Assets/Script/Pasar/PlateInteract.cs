using UnityEngine;

public class PlateInteract : MonoBehaviour
{
    public PlateProgressBar progressBar;
    public GameObject emptyPlatePrefab;
    public Transform spawnPoint;
    public float npcDetectRadius = 2f;

    void Update()
    {
        if (!progressBar.IsFull()) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, npcDetectRadius);
            foreach (Collider col in hits)
            {
                NPCPlateReceiver npc = col.GetComponent<NPCPlateReceiver>();
                if (npc != null)
                {
                    npc.ReceivePlate();
                    SpawnNewPlate();
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    void SpawnNewPlate()
    {
        GameObject newPlate = Instantiate(emptyPlatePrefab, spawnPoint.position, spawnPoint.rotation);

        newPlate.GetComponent<FoodPlate>().progressBar = progressBar;
        newPlate.GetComponent<PlateInteract>().progressBar = progressBar;

        progressBar.ResetBar();
    }
}
