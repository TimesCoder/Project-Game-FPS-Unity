using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayManager : MonoBehaviour
{
    public GameObject loadingImage;
    public GameObject Fire;
    public GameObject Hitpoint;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        StartCoroutine(StartLoading());
    }

    IEnumerator StartLoading()
    {
        loadingImage.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Simpan flag bahwa sudah lewat Main Menu
        PlayerPrefs.SetInt("StartedFromMainMenu", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Scene_A");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }
}
