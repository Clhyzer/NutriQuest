using System.Collections.Generic;
using UnityEngine;

public class NutriScoreManager : MonoBehaviour
{
    [Header("Plate Items")]
    public List<FoodItem> itemsOnPlate = new List<FoodItem>();

    [Header("Score Settings")]
    public int currentScore = 0;
    public int scoreTarget = 100;

    [Header("Portal")]
    public GameObject portalNextLevel;

    public void CalculateScore()
    {
        HashSet<FoodType> healthySet = new HashSet<FoodType>();
        int junkCount = 0;

        foreach (var item in itemsOnPlate)
        {
            switch (item.foodType)
            {
                case FoodType.Karbo:
                case FoodType.Protein:
                case FoodType.Sayur:
                case FoodType.Buah:
                    healthySet.Add(item.foodType); // Mencegah dobel
                    break;

                case FoodType.Junk:
                    junkCount++;
                    break;
            }
        }

        // Perhitungan score
        int healthyScore = healthySet.Count * 5;
        int junkPenalty = junkCount * -5;

        currentScore += healthyScore + junkPenalty;

        Debug.Log("Healthy Score: " + healthyScore +
                  " Junk: -" + junkPenalty +
                  " Total: " + currentScore);

        CheckPortal();
    }

    void CheckPortal()
    {
        if (currentScore >= scoreTarget)
        {
            Debug.Log("Portal unlocked!");
            portalNextLevel.SetActive(true);
        }
    }
}
