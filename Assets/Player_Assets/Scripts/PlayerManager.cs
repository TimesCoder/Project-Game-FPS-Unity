using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject MenuCanvas;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private GameObject aboutPanel;

    [Header("Loading")]
    [SerializeField] private GameObject loadingImage;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowMainMenu();

        // Mainkan BGM menu utama
        AudioManager.Instance.PlayMainMenuBGM();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        MenuCanvas.SetActive(true);
        optionsPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        MenuCanvas.SetActive(false);
        optionsPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }
    public void ShowAbout()
    {
        MenuCanvas.SetActive(false);
        optionsPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void ShowCharacterSelect()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
        aboutPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void PlayGame()
    {
        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        loadingImage.SetActive(true);

        // Ganti ke loading BGM
        AudioManager.Instance.PlayLoadingBGM();

        yield return new WaitForSeconds(2f);

        CleanupSingletonsBeforeLoad();

        PlayerPrefs.SetInt("StartedFromMainMenu", 1);
        PlayerPrefs.Save();

        // Hentikan BGM sebelum pindah scene
        AudioManager.Instance.StopBGM();
        SceneManager.LoadScene("Scene_A");
    }

    private void CleanupSingletonsBeforeLoad()
    {
        if (GameBGMManager.Instance != null)
        {
            Destroy(GameBGMManager.Instance.gameObject);
            GameBGMManager.Instance = null;
        }

        var resumeMenu = Object.FindFirstObjectByType<ResumeMenuController>();
        if (resumeMenu != null)
        {
            Destroy(resumeMenu.gameObject);
            ResumeMenuController.isGamePaused = false;
        }

        var persistentObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in persistentObjects)
        {
            if (obj.scene.name == "DontDestroyOnLoad" && obj.name != "EventSystem" && obj != AudioManager.Instance.gameObject)
            {
                Destroy(obj);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}

