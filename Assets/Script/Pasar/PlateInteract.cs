using UnityEngine;

public class PlateInteract : MonoBehaviour
{
    public PlateProgressBar progressBar;
    public GameObject emptyPlatePrefab;
    public Transform spawnPoint;
    public float npcDetectRadius = 2f;

    void Update()
    {
        if (!progressBar.IsFull()) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryGivePlateToNPC();
        }
    }

    void TryGivePlateToNPC()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, npcDetectRadius);

        foreach (Collider col in hits)
        {
            NPCPlateReceiver npc = col.GetComponent<NPCPlateReceiver>();
            if (npc != null && npc.CanReceivePlate())
            {
                npc.ReceivePlate();
                SpawnNewPlate();
                Destroy(gameObject);
                return;
            }
        }

        Debug.Log("Tidak ada NPC di dekat plate");
    }

    void SpawnNewPlate()
    {
        Instantiate(emptyPlatePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
