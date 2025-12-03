using UnityEngine;
using UnityEngine.UI;

public class FoodScoreSystem : MonoBehaviour
{
    [Header("Score Settings")]
    public int score = 0;
    public int maxScore = 100;
    public Slider scoreBar;

    [Header("Portal Settings")]
    public GameObject portalPrefab;     // prefab portal
    public Transform portalSpawnPoint;  // titik muncul portal
    private bool portalSpawned = false; // untuk mencegah spawn berkali2

    public void AddScore(FoodItem food)
    {
        if (food.foodType == FoodType.Healthy)
        {
            score += 50;
            Debug.Log("[Score] Tambah +10 (Healthy)");
        }
        else if (food.foodType == FoodType.Junk)
        {
            score -= 50;
            Debug.Log("[Score] Kurang -5 (JunkFood)");
        }

        // Batasi score
        score = Mathf.Clamp(score, 0, maxScore);

        // Update UI bar
        if (scoreBar != null)
            scoreBar.value = score;

        // Cek portal
        CheckPortalCondition();
    }

    void CheckPortalCondition()
    {
        // Jika score sudah 100 dan portal belum muncul
        if (!portalSpawned && score >= maxScore)
        {
            SpawnPortal();
        }
    }

    void SpawnPortal()
    {
        if (portalPrefab != null && portalSpawnPoint != null)
        {
            Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);
            Debug.Log("Portal telah muncul!");
        }
        else
        {
            Debug.LogWarning("PortalPrefab atau PortalSpawnPoint belum diassign!");
        }

        portalSpawned = true;
    }
}
