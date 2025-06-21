using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ResumeMenuController : MonoBehaviour
{
    public GameObject resumeMenuPanel;
    public GameObject[] buttons; // Urut: Resume, Restart, Menu, Quit

    private int currentIndex = 0;
    private bool isPaused = false;

    private Color selectedColor = Color.red;
    private Color defaultColor = Color.white;

    void Start()
    {
        resumeMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // Navigasi tombol
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + buttons.Length) % buttons.Length;
                HighlightButton(currentIndex);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % buttons.Length;
                HighlightButton(currentIndex);
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                switch (currentIndex)
                {
                    case 0: ResumeGame(); break;
                    case 1: RestartGame(); break;
                    case 2: BackToMainMenu(); break;
                    case 3: QuitGame(); break;
                }
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        resumeMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        currentIndex = 0;
        HighlightButton(currentIndex);

        // Lock cursor if using mouse-based control
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        resumeMenuPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed");
    }

    void HighlightButton(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Image img = buttons[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == index) ? selectedColor : defaultColor;
        }

        EventSystem.current.SetSelectedGameObject(buttons[index]);
    }
}
