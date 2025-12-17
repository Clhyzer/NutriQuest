using UnityEngine;
using UnityEngine.UI;

public class PlateProgressBar : MonoBehaviour
{
    public Slider slider;
    public float maxValue = 100f;
    public float currentValue = 0f;

    public void AddHealthy(float value)
    {
        currentValue += value;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        slider.value = currentValue / maxValue;
    }

    public void AddJunk(float value)
    {
        currentValue -= value;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        slider.value = currentValue / maxValue;
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
