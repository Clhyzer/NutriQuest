using UnityEngine;

public class NPCPlateReceiver : MonoBehaviour
{
    bool received = false;
    bool playerNear = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }

    public bool CanReceivePlate()
    {
        return playerNear && !received;
    }

    public void ReceivePlate()
    {
        if (received) return;
        received = true;
        GameManager1.Instance.ServeNPC();
        Debug.Log($"{gameObject.name} menerima plate");
    }
} 
