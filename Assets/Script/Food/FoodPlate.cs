using UnityEngine;
using System.Collections.Generic;

public class FoodPlate : MonoBehaviour
{
    public Transform stackPoint;                // titik pusat plate
    public List<FoodItem> foodsOnPlate = new List<FoodItem>();

    private float currentHeight = 0f;           // tinggi tumpukan
    private FoodScoreSystem scoreSystem;

    private void Start()
    {
        scoreSystem = FindObjectOfType<FoodScoreSystem>();
    }

    public void AddFood(FoodItem food)
    {
        foodsOnPlate.Add(food);

        // ⬇️ Tambahkan offset acak untuk X dan Z agar tidak menumpuk di satu titik
        float offsetX = Random.Range(-0.03f, 0.03f);
        float offsetZ = Random.Range(-0.03f, 0.03f);

        Vector3 newPos = stackPoint.position + new Vector3(offsetX, currentHeight, offsetZ);

        // Atur parent dan posisi
        food.transform.SetParent(transform);
        food.transform.position = newPos;
        food.transform.rotation = stackPoint.rotation;

        // Naikkan currentHeight untuk makanan berikutnya
        currentHeight += food.heightOffset;

        // Nonaktifkan physics supaya tidak jatuh
        if (food.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Tambah score otomatis
        if (scoreSystem != null)
        {
            scoreSystem.AddScore(food);
        }
    }

    public void ClearPlate()
    {
        foreach (FoodItem food in foodsOnPlate)
        {
            if (food != null)
                Destroy(food.gameObject);
        }

        foodsOnPlate.Clear();
        currentHeight = 0f;
    }
}
