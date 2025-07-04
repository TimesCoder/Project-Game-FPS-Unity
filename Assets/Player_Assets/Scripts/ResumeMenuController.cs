using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ResumeMenuController : MonoBehaviour
{
    public GameObject resumeMenuPanel;
    public GameObject helpCanvas;
    public GameObject missionCanvas;

    public GameObject[] buttons; // Urut: Resume, Restart, Mission, Help, Quit
    public static bool isGamePaused = false;


    private int currentIndex = 0;
    private bool isPaused = false;

    private Color selectedColor = Color.red;
    private Color defaultColor = Color.white;

    void Start()
    {
        resumeMenuPanel.SetActive(false);
        helpCanvas.SetActive(false);
        missionCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Jika sedang di panel resume, lanjutkan game
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                OpenResumePanel();
            }
        }

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
                    case 2: MissionGame(); break;
                    case 3: HelpGame(); break;
                    case 4: BackToMainMenu(); break;
                }
            }
        }
    }

    void OpenResumePanel()
    {
        isPaused = true;
        resumeMenuPanel.SetActive(true);
        missionCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        Time.timeScale = 0f;
        HighlightButton(currentIndex);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void PauseGame()
    {
        isPaused = true;
        isGamePaused = true; // 🚫 Game paused
        resumeMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        HighlightButton(currentIndex);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        helpCanvas.SetActive(false);
        missionCanvas.SetActive(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        isGamePaused = false; // ✅ Game resume
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

    public void MissionGame()
    {
        currentIndex = 2;

        resumeMenuPanel.SetActive(false);
        missionCanvas.SetActive(true);
        helpCanvas.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HelpGame()
    {
        currentIndex = 3;

        resumeMenuPanel.SetActive(false);
        helpCanvas.SetActive(true);
        missionCanvas.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
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
