using UnityEngine;
using System.Collections.Generic;

public class FoodPlate : MonoBehaviour
{
    public Transform stackPoint;
    public List<FoodItem> foodsOnPlate = new List<FoodItem>();

    [Header("Stack Config")]
    public int slotsPerLayer = 4;
    public float slotSpacing = 0.08f;

    [Header("Progress")]
    public PlateProgressBar progressBar;
    public float healthyValue = 25f;
    public float junkPenalty = 20f;

    private Vector3[] slotOffsets;

    void Start()
    {
        slotOffsets = new Vector3[slotsPerLayer];
        float s = slotSpacing;

        slotOffsets[0] = new Vector3(-s, 0f, -s);
        slotOffsets[1] = new Vector3( s, 0f, -s);
        slotOffsets[2] = new Vector3(-s, 0f,  s);
        slotOffsets[3] = new Vector3( s, 0f,  s);
    }

    public void AddFood(FoodItem food)
    {
        // JUNK → langsung destroy
        if (food.foodType == FoodType.Junk)
        {
            progressBar.AddJunk(junkPenalty);
            Destroy(food.gameObject);
            return;
        }

        // HEALTHY → tambah bar
        progressBar.AddHealthy(healthyValue);

        // MATIKAN PICKUP
        Collider[] cols = food.GetComponentsInChildren<Collider>();
        foreach (var c in cols) c.enabled = false;
        food.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // STACKING
        int index = foodsOnPlate.Count;
        int slotIndex = index % slotsPerLayer;
        int layer = index / slotsPerLayer;

        float height = layer * food.heightOffset;
        Vector3 offset = slotOffsets[slotIndex];
        Vector3 newPos = stackPoint.position + new Vector3(offset.x, height, offset.z);

        Vector3 originalScale = food.transform.lossyScale;
        food.transform.SetParent(transform, true);
        food.transform.position = newPos;
        food.transform.rotation = stackPoint.rotation;

        Vector3 parentScale = transform.lossyScale;
        food.transform.localScale = new Vector3(
            originalScale.x / parentScale.x,
            originalScale.y / parentScale.y,
            originalScale.z / parentScale.z
        );

        foodsOnPlate.Add(food);
    }
}
