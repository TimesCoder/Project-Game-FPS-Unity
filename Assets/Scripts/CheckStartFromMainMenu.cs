using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckStartFromMainMenu : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("StartedFromMainMenu"))
        {
            Debug.Log("Harus mulai dari MainMenu, balik ke MainMenu");
            SceneManager.LoadScene("Main Menu");
        }
        else
        {
            PlayerPrefs.DeleteKey("StartedFromMainMenu");
        }
    }
}
