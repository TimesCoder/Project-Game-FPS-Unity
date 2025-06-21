using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject missionPanel;       // UI untuk mission
    public GameObject resumeMenuPanel;    // UI untuk resume/pause

    private bool isMissionOpen = false;
    private bool isResumeMenuOpen = false;

    public static class InputManager
    {
        public static bool isPauseMenuActive = false;
    }

    void Update()
    {
        // Tekan ESC untuk toggle Resume Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isResumeMenuOpen = !isResumeMenuOpen;
            resumeMenuPanel.SetActive(isResumeMenuOpen);

            // Atur status game dan input global
            Time.timeScale = isResumeMenuOpen ? 0f : 1f;
            InputManager.isPauseMenuActive = isResumeMenuOpen;

            // Saat resume menu dibuka, pastikan mission panel tertutup
            if (isResumeMenuOpen && missionPanel.activeSelf)
            {
                missionPanel.SetActive(false);
                isMissionOpen = false;
            }

            return; // Hentikan eksekusi update agar Tab tidak bisa ditekan saat ESC ditekan
        }

        // Jika Pause Menu aktif, abaikan input selain navigasi resume
        if (InputManager.isPauseMenuActive)
        {
            return;
        }

        // Tekan TAB untuk toggle Mission Panel
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isMissionOpen = !isMissionOpen;
            missionPanel.SetActive(isMissionOpen);
        }
    }
}
