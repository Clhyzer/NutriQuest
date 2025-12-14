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
    public PlayerMotor playerMovement;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip countdownTickSFX; // 3,2,1
    public AudioClip goSFX;            // GO!

    void Start()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        startPanel.SetActive(true);
        countdownPanel.SetActive(false);

        StartCoroutine(StartSequence());
    }

    System.Collections.IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(1f);

        startPanel.SetActive(false);
        countdownPanel.SetActive(true);

        float remaining = countdownTime;

        while (remaining > 0)
        {
            countdownText.text = Mathf.Ceil(remaining).ToString();

            if (audioSource && countdownTickSFX)
                audioSource.PlayOneShot(countdownTickSFX);

            yield return new WaitForSeconds(1f);
            remaining--;
        }

        // GO!
        countdownText.text = "GO!";

        if (audioSource && goSFX)
            audioSource.PlayOneShot(goSFX);

        yield return new WaitForSeconds(0.5f);

        countdownPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        GameManager.Instance.StartGame();
    }
}
