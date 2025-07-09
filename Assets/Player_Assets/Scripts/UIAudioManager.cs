using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    [Header("Click Sound")]
    public AudioClip buttonClickClip;
    public float volume = 1.0f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // optional: persist across scenes
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        if (buttonClickClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickClip, volume);
        }
    }
}
