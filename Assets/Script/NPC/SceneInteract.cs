using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInteract : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
