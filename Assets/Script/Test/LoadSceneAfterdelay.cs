using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterDelay : MonoBehaviour
{
    public string sceneName;
    public float delay = 3f;

    void Start()
    {
        Invoke(nameof(LoadScene), delay);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
