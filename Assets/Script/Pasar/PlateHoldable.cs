using UnityEngine;

public class PlateHoldable : MonoBehaviour
{
    public PlateProgressBar progressBar;

    public bool CanBePicked()
    {
        return progressBar != null && progressBar.IsFull();
    }
}
