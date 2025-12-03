using System.Collections.Generic;
using UnityEngine;

public class PlateZone : MonoBehaviour
{
    public NutriScoreManager scoreManager;    // HUBUNGAN dgn NutriScoreManager

    private List<GameObject> itemsOnPlate = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        FoodItem food = other.GetComponent<FoodItem>();
        if (food == null) return;

        // Kalau makanan belum masuk plate
        if (!itemsOnPlate.Contains(other.gameObject))
        {
            itemsOnPlate.Add(other.gameObject);

            // masukkan ke sistem scoring
            scoreManager.itemsOnPlate.Add(food);

            // hitung score tiap makanan masuk
            scoreManager.CalculateScore();

            Debug.Log("Makanan masuk ke plate: " + food.foodType);
        }
    }

    void OnTriggerExit(Collider other)
    {
        FoodItem food = other.GetComponent<FoodItem>();
        if (food == null) return;

        // Jika makanan keluar dari plate
        if (itemsOnPlate.Contains(other.gameObject))
        {
            itemsOnPlate.Remove(other.gameObject);
            scoreManager.itemsOnPlate.Remove(food);

            Debug.Log("Makanan keluar dari plate.");
        }
    }
}
