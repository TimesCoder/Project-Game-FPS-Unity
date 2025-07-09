using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBGMManager : MonoBehaviour
{
    public static GameBGMManager Instance;

    [Header("BGM Settings")]
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool loop = true;

    [Header("Scene Filter")]
    public string sceneToPlay = "Scene_A"; // Nama scene yang boleh play BGM

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        SetupAudioSource();

        // Play jika scene saat ini cocok
        if (SceneManager.GetActiveScene().name == sceneToPlay)
        {
            PlayBackgroundMusic();
        }
    }

    private void SetupAudioSource()
    {
        audioSource.clip = backgroundMusic;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
    }

    public void PlayBackgroundMusic()
    {
        if (audioSource == null || backgroundMusic == null)
            return;

        if (!audioSource.isPlaying)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null; // hentikan buffer audio
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.name != sceneToPlay)
        {
            StopMusic();
        }
        else
        {
            SetupAudioSource(); // pastikan audioSource siap
            PlayBackgroundMusic();
        }
    }

    private void OnApplicationQuit()
    {
        StopMusic();
    }
}
