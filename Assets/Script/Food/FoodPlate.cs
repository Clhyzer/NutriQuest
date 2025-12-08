using UnityEngine;
using System.Collections.Generic;

public class FoodPlate : MonoBehaviour
{
    public Transform stackPoint;                // titik pusat plate
    public List<FoodItem> foodsOnPlate = new List<FoodItem>();

    private float currentHeight = 0f;           // tinggi tumpukan
    private FoodScoreSystem scoreSystem;

    // new: konfigurasi slot
    public int slotsPerLayer = 4;               // 4 titik per layer
    public float slotSpacing = 0.08f;           // jarak dari pusat untuk tiap slot
    private Vector3[] slotOffsets;
    
    [Header("Capacity")]
    [Tooltip("Maximum number of items on the plate before it clears automatically")]
    public int maxItems = 4;

    private void Start()
    {
        scoreSystem = FindObjectOfType<FoodScoreSystem>();

        // siapkan 4 offset (kiri-belakang, kanan-belakang, kiri-depan, kanan-depan)
        slotOffsets = new Vector3[slotsPerLayer];
        float s = slotSpacing;
        slotOffsets[0] = new Vector3(-s, 0f, -s);
        slotOffsets[1] = new Vector3( s, 0f, -s);
        slotOffsets[2] = new Vector3(-s, 0f,  s);
        slotOffsets[3] = new Vector3( s, 0f,  s);
    }

    public void AddFood(FoodItem food)
    {
        // index dan layer berdasarkan jumlah saat ini (belum termasuk yang akan ditambahkan)
        int index = foodsOnPlate.Count;
        int slotIndex = index % slotsPerLayer;
        int layer = index / slotsPerLayer;

        // hitung tinggi untuk layer ini
        float height = layer * food.heightOffset;

        // hitung posisi final relatif ke stackPoint
        Vector3 offset = slotOffsets[slotIndex];
        Vector3 newPos = stackPoint.position + new Vector3(offset.x, height, offset.z);

        // simpan world scale asli agar tidak ter-stretch saat di-parent
        Vector3 originalWorldScale = food.transform.lossyScale;

        // parent dan set posisi/rotasi
        food.transform.SetParent(transform, true); // pertahankan world transform sementara
        food.transform.position = newPos;
        food.transform.rotation = stackPoint.rotation;

        // sesuaikan localScale sehingga tetap sama di world space meski parent punya skala
        Vector3 parentLossy = transform.lossyScale;
        Vector3 newLocalScale = Vector3.one;
        newLocalScale.x = parentLossy.x != 0f ? originalWorldScale.x / parentLossy.x : originalWorldScale.x;
        newLocalScale.y = parentLossy.y != 0f ? originalWorldScale.y / parentLossy.y : originalWorldScale.y;
        newLocalScale.z = parentLossy.z != 0f ? originalWorldScale.z / parentLossy.z : originalWorldScale.z;
        food.transform.localScale = newLocalScale;

        // tambahkan ke list setelah pos ditentukan
        foodsOnPlate.Add(food);

        Debug.Log($"FoodPlate.AddFood: added '{food.name}' -> count={foodsOnPlate.Count}");

        // update currentHeight optional (dipakai hanya untuk referensi)
        currentHeight = (layer + 1) * food.heightOffset;

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

        // Jika jumlah item tepat mencapai batas, hapus semua item di plate
        if (foodsOnPlate.Count == maxItems)
        {
            ClearPlate();
        }
    }

    public void ClearPlate()
    {
        Debug.Log($"FoodPlate.ClearPlate: clearing {foodsOnPlate.Count} items");
        for (int i = foodsOnPlate.Count - 1; i >= 0; i--)
        {
            FoodItem food = foodsOnPlate[i];
            if (food != null)
            {
                // Pastikan menghancurkan root GameObject dari item (jika FoodItem berada di child)
                GameObject root = food.transform.root != null ? food.transform.root.gameObject : food.gameObject;
                Debug.Log($" - Destroying root '{root.name}' for component '{food.name}'");
                if (Application.isPlaying)
                    Destroy(root);
                else
                    DestroyImmediate(root);
            }
        }

        foodsOnPlate.Clear();
        currentHeight = 0f;
    }
}