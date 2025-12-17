using UnityEngine;
using UnityEngine.AI;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Prefabs (Random)")]
    public GameObject[] npcPrefabs;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public int spawnAtOnce = 3;
    public float spawnInterval = 8f;
    public int maxNPC = 10;

    [Header("Waypoint (Scene)")]
    public WaypointNode[] sceneWaypoints;   // 👈 TAMBAHAN PENTING

    private int currentNPC = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnWave), 1f, spawnInterval);
    }

    void SpawnWave()
    {
        if (currentNPC >= maxNPC) return;

        int spawnCount = Mathf.Min(spawnAtOnce, maxNPC - currentNPC);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnSingleNPC();
        }
    }

    void SpawnSingleNPC()
    {
        if (npcPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject prefab =
            npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        Transform spawn =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawn.position, out hit, 2f, NavMesh.AllAreas))
        {
            GameObject npc = Instantiate(prefab, hit.position, spawn.rotation);
            currentNPC++;

            // 🔥 ASSIGN WAYPOINT SETELAH SPAWN
            RandoomNPC npcScript = npc.GetComponent<RandoomNPC>();
            if (npcScript != null)
            {
                npcScript.waypoint = sceneWaypoints;
            }

            // listener agar counter aman
            npc.AddComponent<NPCDespawnListener>().Init(this);
        }
    }

    public void OnNPCDestroyed()
    {
        currentNPC = Mathf.Max(0, currentNPC - 1);
    }
}
