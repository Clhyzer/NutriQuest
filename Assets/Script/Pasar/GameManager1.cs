using UnityEngine;
using TMPro;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;
    public int npcTarget = 1;
    public int npcServed = 0;
    public TextMeshProUGUI questText;

    void Awake()
    {
        Instance = this;
        UpdateText();
    }

    public void ServeNPC()
    {
        npcServed++;
        UpdateText();
    }

    void UpdateText()
    {
        questText.text = $"Serve NPC: {npcServed}/{npcTarget}";
    }
}
