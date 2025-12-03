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

    [Header("UI")]
    public Slider barSlider;
    public Text timeText;
    public GameObject endGameUI;
    public Text endTimeText;
    public Text endPointText;

    private float timer;
    private int score;
    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timer = timeLimit;
        barSlider.maxValue = maxBarValue;
        barSlider.value = 0;
        endGameUI.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying) return;

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
        if (!isPlaying) return;

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

    void FinishLevel(bool success)
    {
        isPlaying = false;
        endGameUI.SetActive(true);

        endTimeText.text = "Sisa Waktu : " + timer.ToString("F1");
        endPointText.text = "Point : " + score;

        StartCoroutine(LevelDelay(success));
    }

    System.Collections.IEnumerator LevelDelay(bool success)
    {
        yield return new WaitForSeconds(delayAfterFinish);

        if (success)
            NextLevel();
        else
            Retry();
    }

    void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
