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

    [Header("Loading")]
    [SerializeField] private GameObject loadingImage;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        MenuCanvas.SetActive(true);
        optionsPanel.SetActive(false);
        characterSelectPanel.SetActive(false);
    }

    public void ShowOptions()
    {
        MenuCanvas.SetActive(false);
        optionsPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
    }

    public void ShowCharacterSelect()
    {
        mainMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu(); // tombol "Back" akan panggil ini
    }

    public void PlayGame()
    {
        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        loadingImage.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Simpan bahwa kita mulai dari menu utama
        PlayerPrefs.SetInt("StartedFromMainMenu", 1);
        PlayerPrefs.Save();

        // Tetap load ke Scene_A, tidak peduli karakter apa
        SceneManager.LoadScene("Scene_A");
    }


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}
