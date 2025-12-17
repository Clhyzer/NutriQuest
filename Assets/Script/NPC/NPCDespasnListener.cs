using UnityEngine;

public class NPCDespawnListener : MonoBehaviour
{
    private NPCSpawner spawner;

    public void Init(NPCSpawner s)
    {
        spawner = s;
    }

    void OnDestroy()
    {
        if (spawner != null)
            spawner.OnNPCDestroyed();
    }
}
