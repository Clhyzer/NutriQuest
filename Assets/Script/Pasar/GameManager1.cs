using UnityEngine;
using TMPro;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;

    [Header("Quest")]
    public int npcTarget = 1;
    public int npcServed = 0;
    public TextMeshProUGUI questText;

    [Header("Portal")]
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;

    private bool portalSpawned = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameManager1 initialized");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateText();
        
        // DEBUG: Cek setup di Awake
        if (portalPrefab == null)
            Debug.LogError("❌ PORTAL PREFAB TIDAK DI-ASSIGN di Inspector!");
        else
            Debug.Log($"✓ Portal Prefab: {portalPrefab.name}");
            
        if (portalSpawnPoint == null)
            Debug.LogError("❌ PORTAL SPAWN POINT TIDAK DI-ASSIGN di Inspector!");
        else
            Debug.Log($"✓ Portal Spawn Point: {portalSpawnPoint.name} at {portalSpawnPoint.position}");
            
        Debug.Log($"Quest Target: {npcTarget}");
    }

    public void ServeNPC()
    {
        npcServed++;
        Debug.Log($"🍽️ NPC Served! Progress: {npcServed}/{npcTarget}");
        
        UpdateText();
        CheckQuestComplete();
    }

    void UpdateText()
    {
        if (questText != null)
            questText.text = $"Serve NPC: {npcServed}/{npcTarget}";
        else
            Debug.LogWarning("Quest Text UI belum di-assign!");
    }

    void CheckQuestComplete()
    {
        Debug.Log($"Checking quest: served={npcServed}, target={npcTarget}, portalSpawned={portalSpawned}");
        
        if (npcServed >= npcTarget && !portalSpawned)
        {
            Debug.Log("✅ QUEST COMPLETE! Spawning portal...");
            SpawnPortal();
            portalSpawned = true;
        }
        else if (portalSpawned)
        {
            Debug.Log("Portal sudah di-spawn sebelumnya");
        }
        else
        {
            Debug.Log($"Quest belum selesai: {npcServed}/{npcTarget}");
        }
    }

    void SpawnPortal()
    {
        if (portalPrefab == null)
        {
            Debug.LogError("❌ Portal Prefab NULL! Tidak bisa spawn portal!");
            return;
        }
        
        if (portalSpawnPoint == null)
        {
            Debug.LogError("❌ Portal Spawn Point NULL! Spawn di posisi (0,0,0)");
            GameObject portal = Instantiate(portalPrefab);
            Debug.Log($"Portal spawned at default position: {portal.transform.position}");
            return;
        }

        GameObject spawnedPortal = Instantiate(
            portalPrefab,
            portalSpawnPoint.position,
            portalSpawnPoint.rotation
        );

        Debug.Log($"🌀 PORTAL SPAWNED! Name: {spawnedPortal.name}, Position: {spawnedPortal.transform.position}");
        
        // Cek apakah portal aktif
        if (!spawnedPortal.activeSelf)
        {
            Debug.LogWarning("⚠️ Portal prefab tidak aktif! Mengaktifkan...");
            spawnedPortal.SetActive(true);
        }
    }
    
    // METHOD UNTUK TESTING DI INSPECTOR / CONSOLE
    [ContextMenu("Test Serve NPC")]
    void TestServeNPC()
    {
        ServeNPC();
    }
    
    [ContextMenu("Force Spawn Portal")]
    void ForceSpawnPortal()
    {
        SpawnPortal();
    }
    
    [ContextMenu("Reset Quest")]
    void ResetQuest()
    {
        npcServed = 0;
        portalSpawned = false;
        UpdateText();
        Debug.Log("Quest direset!");
    }
}