using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public AudioClip clickSFX;
    [Range(0f, 3f)] public float volume = 1.5f;

    public void PlayClick()
    {
        if (clickSFX == null) return;

        AudioSource.PlayClipAtPoint(clickSFX, Vector3.zero, volume);
    }
}
