using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gameplay Settings")]
    public float maxBarValue = 100;
    public float currentBarValue = 0;
    public float timeLimit = 30f;
    public float healthyGain = 20f;
    public float junkLoss = 15f;

    [Header("Delay Settings")]
    public float delayAfterFinish = 2f;

    [Header("UI - In Game")]
    public Slider barSlider;
    public Text timeText;

    [Header("UI - WIN")]
    public GameObject winPanel;
    public Text winTimeText;
    public Text winPointText;

    [Header("UI - LOSE")]
    public GameObject losePanel;
    public Text loseTimeText;
    public Text losePointText;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip winSFX;
    public AudioClip loseSFX;

    private float timer;
    private int score;
    private bool isPlaying = false;
    private bool gameFinished = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timer = timeLimit;
        barSlider.maxValue = maxBarValue;
        barSlider.value = 0;

        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying || gameFinished) return;

        timer -= Time.deltaTime;
        timeText.text = "Time: " + timer.ToString("F1");

        if (currentBarValue >= maxBarValue)
            FinishLevel(true);

        if (timer <= 0)
            FinishLevel(false);
    }

    public void StartGame()
    {
        isPlaying = true;
    }

    public void CollectFood(Fooditemm food)
    {
        if (!isPlaying || gameFinished) return;

        if (food.foodType == FoodTP.Healthy)
        {
            currentBarValue += healthyGain;
            score += 10;
        }
        else
        {
            currentBarValue -= junkLoss;
            if (currentBarValue < 0) currentBarValue = 0;
        }

        barSlider.value = currentBarValue;
    }

    void FinishLevel(bool win)
    {
        gameFinished = true;
        isPlaying = false;

        if (win)
        {
            winPanel.SetActive(true);
            winTimeText.text = "Sisa Waktu : " + timer.ToString("F1");
            winPointText.text = "Point : " + score;

            PlaySFX(winSFX);
        }
        else
        {
            losePanel.SetActive(true);
            loseTimeText.text = "Waktu Habis!";
            losePointText.text = "Point : " + score;

            PlaySFX(loseSFX);
        }

        StartCoroutine(LevelDelay(win));
    }

    void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clip);
        }
    }

    System.Collections.IEnumerator LevelDelay(bool win)
    {
        yield return new WaitForSeconds(delayAfterFinish);

        if (win)
            NextLevel();
        else
            Retry();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
