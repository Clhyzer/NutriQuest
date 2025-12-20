using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;

    [Header("UI")]
    public GameObject pausePanel;

    [Header("Settings")]
    public string homeSceneName = "MainMenu";

    void Start()
    {
        Resume(); // pastikan game jalan saat start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;

        // Kursor kembali ke mode FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;

        // Kursor bebas untuk klik UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(homeSceneName);
    }
}
