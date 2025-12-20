using UnityEngine;

public class FoodPickupSource : MonoBehaviour
{
    [Header("Prefab yang akan dipegang player")]
    public GameObject foodPrefab;

    [Header("Optional")]
    public float pickupCooldown = 0f;

    private bool canPickup = true;

    public GameObject SpawnFood()
    {
        if (!canPickup) return null;

        if (foodPrefab == null)
        {
            Debug.LogError($"{name} : Food prefab belum di-assign!");
            return null;
        }

        if (pickupCooldown > 0)
            StartCoroutine(Cooldown());

        GameObject food = Instantiate(foodPrefab);
        return food;
    }

    System.Collections.IEnumerator Cooldown()
    {
        canPickup = false;
        yield return new WaitForSeconds(pickupCooldown);
        canPickup = true;
    }
}
