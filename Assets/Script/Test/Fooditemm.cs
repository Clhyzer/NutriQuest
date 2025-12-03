using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FoodTP { Healthy, Junk }

public class Fooditemm : MonoBehaviour
{
    public FoodTP foodType;

    [Header("Highlight")]
    public GameObject highlightBorder; // drag border object

    public void SetHighlight(bool active)
    {
        if (highlightBorder != null)
            highlightBorder.SetActive(active);
    }
}
