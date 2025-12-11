using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public string exitSceneName = "MainMenu";

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private bool isGameOver = false;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (backgroundMusic != null)
            backgroundMusic.Stop();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
    }

    public void OnRetry()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 🔥 Pindah Scene
    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(sceneName);
    }

    // 🔥 EXIT GAME (KELUAR DARI APLIKASI)
    public void ExitGame()
    {
        Debug.Log("EXIT GAME");

        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
