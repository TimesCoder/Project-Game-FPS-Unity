using UnityEngine;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip mainMenuBGM;
    [SerializeField] private AudioClip loadingBGM;
    [Range(0f, 1f)] public float volume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMainMenuBGM()
    {
        if (audioSource.clip == mainMenuBGM && audioSource.isPlaying) return;

        audioSource.clip = mainMenuBGM;
        audioSource.Play();
    }

    public void PlayLoadingBGM()
    {
        if (audioSource.clip == loadingBGM && audioSource.isPlaying) return;

        audioSource.clip = loadingBGM;
        audioSource.Play();
    }

    public void StopBGM(float fadeDuration = 1f)
    {
        if (audioSource.isPlaying)
            StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeOut(float duration)
    {
        float startVol = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVol * Time.unscaledDeltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
    }

}