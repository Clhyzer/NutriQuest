using UnityEngine;

public class NPCPlateReceiver : MonoBehaviour
{
    private bool playerNear = false;

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
        return playerNear;
    }

    public void ReceivePlate()
    {
        Debug.Log($"{gameObject.name} menerima plate");

        GameManager1.Instance.GivePlateToNPC();
    }
}
