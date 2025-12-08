using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn objek secara acak ke beberapa titik spawn setiap kali scene dimulai.
/// - Atur array `prefabs` dan `spawnPoints` di Inspector.
/// - Gunakan `spawnCount`, `uniquePoints`, `jitterRadius` dan opsi lainnya untuk mengontrol perilaku.
/// - Panggil `SpawnNow()` dari skrip lain atau centang `spawnOnStart` supaya otomatis spawn di Start().
/// </summary>
public class RandomSpawn : MonoBehaviour
{
    public enum RotationMode
    {
        MatchSpawnPoint,
        UsePrefabRotation,
        Zero
    }

    [Header("Rotation")]
    [Tooltip("How to set rotation for spawned objects")]
    public RotationMode rotationMode = RotationMode.MatchSpawnPoint;

    [Header("Prefabs to spawn")]
    public GameObject[] prefabs;

    [Header("Spawn points")]
    public Transform[] spawnPoints;

    [Tooltip("How many objects to spawn each time")]
    public int spawnCount = 1;

    [Tooltip("If true, no two spawned objects will use the same spawn point")]
    public bool uniquePoints = true;

    [Tooltip("If true, pick a random prefab per spawn; otherwise use prefabs[0]")]
    public bool randomPrefabPerPoint = true;

    [Tooltip("Parent spawned objects under this GameObject")]
    public bool parentToThis = true;

    [Header("Timing")]
    [Tooltip("If true, spawn automatically in Start()")]
    public bool spawnOnStart = true;

    [Header("Position variation")]
    [Tooltip("Max random offset (meters) applied to spawn position around the spawn point")]
    public float jitterRadius = 0f;

    [Header("Cleanup")]
    [Tooltip("If true, destroy previously spawned children under this GameObject before spawning")]
    public bool clearPreviousChildren = false;

    void OnValidate()
    {
        if (spawnCount < 0) spawnCount = 0;
        if (prefabs == null) prefabs = new GameObject[0];
        if (spawnPoints == null) spawnPoints = new Transform[0];
        if (jitterRadius < 0f) jitterRadius = 0f;
        if (uniquePoints && spawnPoints != null)
        {
            spawnCount = Mathf.Clamp(spawnCount, 0, spawnPoints.Length);
        }
    }

    void Start()
    {
        // Seed Unity's RNG using time so each scene start differs
        int seed = System.Environment.TickCount ^ (int)(System.DateTime.Now.Ticks & 0xFFFFFFFF);
        UnityEngine.Random.InitState(seed);

        if (spawnOnStart)
            SpawnNow();
    }

    /// <summary>
    /// Spawn objects according to current settings.
    /// Can be called from other scripts or via the Inspector (custom editor).
    /// </summary>
    public void SpawnNow()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("RandomSpawn: no prefabs assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("RandomSpawn: no spawn points assigned.");
            return;
        }

        if (clearPreviousChildren && parentToThis)
        {
            // Destroy immediate children
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }
        }

        int availablePoints = spawnPoints.Length;

        if (uniquePoints)
            spawnCount = Mathf.Clamp(spawnCount, 0, availablePoints);
        else
            spawnCount = Mathf.Max(0, spawnCount);

        // Prepare indices for unique selection
        List<int> indices = new List<int>(availablePoints);
        for (int i = 0; i < availablePoints; i++) indices.Add(i);
        if (uniquePoints) Shuffle(indices);

        for (int i = 0; i < spawnCount; i++)
        {
            int pointIndex = uniquePoints ? indices[i] : UnityEngine.Random.Range(0, availablePoints);
            Transform spawnPoint = spawnPoints[pointIndex];
            if (spawnPoint == null) continue;

            GameObject chosenPrefab = randomPrefabPerPoint ? prefabs[UnityEngine.Random.Range(0, prefabs.Length)] : prefabs[0];
            if (chosenPrefab == null)
            {
                Debug.LogWarning("RandomSpawn: one of the prefabs is null, skipping.");
                continue;
            }

            Vector3 pos = spawnPoint.position;
            if (jitterRadius > 0f)
            {
                Vector2 r = UnityEngine.Random.insideUnitCircle * jitterRadius;
                pos += new Vector3(r.x, 0f, r.y);
            }

            Quaternion rot;
            switch (rotationMode)
            {
                case RotationMode.UsePrefabRotation:
                    rot = chosenPrefab.transform.rotation;
                    break;
                case RotationMode.Zero:
                    rot = Quaternion.identity;
                    break;
                default:
                    rot = spawnPoint.rotation;
                    break;
            }

            if (parentToThis)
                Instantiate(chosenPrefab, pos, rot, this.transform);
            else
                Instantiate(chosenPrefab, pos, rot);
        }
    }

    // Fisher-Yates shuffle
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}

