using UnityEngine;
using TMPro;

public class StartCountdown : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject countdownPanel;

    [Header("Countdown")]
    public TMP_Text countdownText;
    public float countdownTime = 3f;

    [Header("Player")]
    public PlayerMotor playerMovement;  // <-- drag script movement ke sini

    void Start()
    {
        // Nonaktifkan movement saat start & countdown
        if (playerMovement != null)
            playerMovement.enabled = false;

        startPanel.SetActive(true);
        countdownPanel.SetActive(false);

        StartCoroutine(StartSequence());
    }

    System.Collections.IEnumerator StartSequence()
    {
        // Start panel muncul 1 detik
        yield return new WaitForSeconds(1f);

        // Masuk countdown
        startPanel.SetActive(false);
        countdownPanel.SetActive(true);

        float remaining = countdownTime;

        while (remaining > 0)
        {
            countdownText.text = Mathf.Ceil(remaining).ToString();
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        // GO!
        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        // Sembunyikan panel countdown
        countdownPanel.SetActive(false);

        // Aktifkan movement setelah countdown selesai
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Mulai Gameplay
        GameManager.Instance.StartGame();
    }
}
