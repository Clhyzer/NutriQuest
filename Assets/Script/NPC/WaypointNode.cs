using UnityEngine;

public enum WaypointType
{
    Walk,
    Shop,
    Cashier,
    Exit
}

public class WaypointNode : MonoBehaviour
{
    public WaypointType type = WaypointType.Walk;
    public float stopDuration = 2f;
}
