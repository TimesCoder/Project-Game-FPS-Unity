using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject missionPanel;
    public GameObject helpPanel;
    public GameObject resumeMenuPanel;

    private bool isMissionOpen = false;
    private bool isHelpOpen = false;
    private bool isResumeMenuOpen = false;

    public static class InputManager
    {
        public static bool isPauseMenuActive = false;
    }

    void Update()
    {
        // ESC: buka/tutup Resume Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isResumeMenuOpen = !isResumeMenuOpen;
            resumeMenuPanel.SetActive(isResumeMenuOpen);

            Time.timeScale = isResumeMenuOpen ? 0f : 1f;
            InputManager.isPauseMenuActive = isResumeMenuOpen;

            // Tutup panel lain
            CloseHelpPanel();
            CloseMissionPanel();

            return;
        }

        if (resumeMenuPanel.activeSelf) return;

        // TAB toggle Mission
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isMissionOpen)
                CloseMissionPanel();
            else
            {
                OpenMissionPanel();
                CloseHelpPanel();
            }
        }

        // I toggle Help
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isHelpOpen)
                CloseHelpPanel();
            else
            {
                OpenHelpPanel();
                CloseMissionPanel();
            }
        }
    }

    public void OpenMissionPanel()
    {
        missionPanel.SetActive(true);
        isMissionOpen = true;
    }

    public void CloseMissionPanel()
    {
        missionPanel.SetActive(false);
        isMissionOpen = false;
    }

    public void OpenHelpPanel()
    {
        helpPanel.SetActive(true);
        isHelpOpen = true;
    }

    public void CloseHelpPanel()
    {
        helpPanel.SetActive(false);
        isHelpOpen = false;
    }
}
