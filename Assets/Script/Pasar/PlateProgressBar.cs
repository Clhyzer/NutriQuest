using UnityEngine;
using UnityEngine.UI;

public class PlateProgressBar : MonoBehaviour
{
    public Slider slider;
    public float maxValue = 100f;
    public float currentValue = 0f;

    void Awake()
    {
        slider.value = 0;
    }

    public void AddHealthy(float value)
    {
        currentValue += value;
        UpdateBar();
    }

    public void AddJunk(float value)
    {
        currentValue -= value;
        UpdateBar();
    }

    void UpdateBar()
    {
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        slider.value = currentValue / maxValue;
        Debug.Log("BAR: " + currentValue);
    }

    public bool IsFull()
    {
        return currentValue >= maxValue;
    }

    public void ResetBar()
    {
        currentValue = 0;
        slider.value = 0;
    }
}
