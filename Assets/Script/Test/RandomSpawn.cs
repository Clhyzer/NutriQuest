using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public enum RotationMode
    {
        MatchSpawnPoint,
        UsePrefabRotation,
        Zero
    }

    [Header("Rotation")]
    public RotationMode rotationMode = RotationMode.MatchSpawnPoint;

    [Header("Healthy Prefabs")]
    public GameObject[] healthyPrefabs;

    [Header("Junk Prefabs")]
    public GameObject[] junkPrefabs;

    [Header("Spawn points")]
    public Transform[] spawnPoints;

    [Tooltip("How many objects to spawn")]
    public int spawnCount = 1;

    [Tooltip("If true, no two spawned objects use the same spawn point")]
    public bool uniquePoints = true;

    [Tooltip("Parent spawned objects under this GameObject")]
    public bool parentToThis = true;

    [Header("Position variation")]
    public float jitterRadius = 0f;

    [Header("Timing")]
    public bool spawnOnStart = true;

    void Start()
    {
        if (spawnOnStart)
            SpawnNow();
    }

    public void SpawnNow()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned.");
            return;
        }

        if (healthyPrefabs.Length == 0 || junkPrefabs.Length == 0)
        {
            Debug.LogWarning("Healthy or Junk prefabs not assigned.");
            return;
        }

        int count = Mathf.Min(spawnCount, spawnPoints.Length);

        // 🔹 Buat list tipe spawn (50:50)
        List<bool> spawnTypes = new List<bool>();
        for (int i = 0; i < count; i++)
            spawnTypes.Add(i < count / 2); // true = healthy

        Shuffle(spawnTypes);

        // 🔹 Acak spawn point
        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            indices.Add(i);

        if (uniquePoints)
            Shuffle(indices);

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[indices[i]];
            if (point == null) continue;

            GameObject prefab = spawnTypes[i]
                ? healthyPrefabs[Random.Range(0, healthyPrefabs.Length)]
                : junkPrefabs[Random.Range(0, junkPrefabs.Length)];

            Vector3 pos = point.position;
            if (jitterRadius > 0)
            {
                Vector2 r = Random.insideUnitCircle * jitterRadius;
                pos += new Vector3(r.x, 0, r.y);
            }

            Quaternion rot = rotationMode switch
            {
                RotationMode.UsePrefabRotation => prefab.transform.rotation,
                RotationMode.Zero => Quaternion.identity,
                _ => point.rotation
            };

            if (parentToThis)
                Instantiate(prefab, pos, rot, transform);
            else
                Instantiate(prefab, pos, rot);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
