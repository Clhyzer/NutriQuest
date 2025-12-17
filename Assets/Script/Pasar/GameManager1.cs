using UnityEngine;
using TMPro;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance;

    public int npcTarget = 1;
    public int npcServed = 0;
    public TextMeshProUGUI questText;

    private void Awake()
    {
        Instance = this;
        UpdateQuestText();
    }

    public void GivePlateToNPC()
    {
        npcServed++;
        UpdateQuestText();
    }

    void UpdateQuestText()
    {
        questText.text = $"Serve NPC: {npcServed}/{npcTarget}";
    }
}
