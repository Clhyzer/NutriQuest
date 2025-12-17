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
            Instance = this;
        else
            Destroy(gameObject);

        UpdateText();
    }

    public void ServeNPC()
    {
        npcServed++;
        UpdateText();

        CheckQuestComplete();
    }

    void UpdateText()
    {
        if (questText != null)
            questText.text = $"Serve NPC: {npcServed}/{npcTarget}";
    }

    void CheckQuestComplete()
    {
        if (npcServed >= npcTarget && !portalSpawned)
        {
            SpawnPortal();
            portalSpawned = true;
        }
    }

    void SpawnPortal()
    {
        if (portalPrefab == null || portalSpawnPoint == null)
        {
            Debug.LogError("Portal Prefab atau Spawn Point BELUM DIISI!");
            return;
        }

        Instantiate(
            portalPrefab,
            portalSpawnPoint.position,
            portalSpawnPoint.rotation
        );

        Debug.Log("QUEST SELESAI - Portal Muncul!");
    }
}
