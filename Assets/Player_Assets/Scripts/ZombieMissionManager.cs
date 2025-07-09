using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ZombieMissionManager : MonoBehaviour
{
    public static ZombieMissionManager Instance;

    [Header("UI")]
    public Text timerText;
    public Text killText;
    public GameObject allMissionCompleteUI;
    public Text missionCompletePopup;

    [Header("Winner UI")]
    public GameObject winnerPanel;
    public Button backToMenuButton;
    public Text backToMenuButtonText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip missionCompleteClip;
    public AudioClip allMissionsDoneClip;

    [Header("Crate Settings")]
    public int totalCrates = 3;

    [Header("Zombie Kill Settings")]
    public int zombieTarget = 20;

    private int zombiesKilled = 0;
    private int cratesCollected = 0;

    private bool misi1Selesai = false;
    private bool misi2Selesai = false;
    private bool misi3Selesai = false;

    private float waktuMisi1 = 60f;
    private float waktuMisi1Mulai = 0f;

    private bool alreadyFinished = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (allMissionCompleteUI != null) allMissionCompleteUI.SetActive(false);
        if (missionCompletePopup != null) missionCompletePopup.text = "";
        if (winnerPanel != null) winnerPanel.SetActive(false);

        MissionsComplete.occurrence.GetMissionsDone(false, false, false);
        waktuMisi1Mulai = Time.time;

        StartCoroutine(CheckMisi1_Bertahan());
        StartCoroutine(CheckMisi2_BunuhZombie());
        StartCoroutine(CheckMisi3_CariFuel_Tank());
    }

    IEnumerator CheckMisi1_Bertahan()
    {
        while (!misi1Selesai)
        {
            float waktuBerjalan = Time.time - waktuMisi1Mulai;
            float sisaWaktu = Mathf.Max(0f, waktuMisi1 - waktuBerjalan);
            timerText.text = $"Waktu: {Mathf.CeilToInt(sisaWaktu)}s";

            if (sisaWaktu <= 0f)
            {
                misi1Selesai = true;
                if (timerText != null)
                    timerText.gameObject.SetActive(false);

                HandleMissionComplete(1);
            }

            if (HealthController_Player.Instance != null && HealthController_Player.Instance.IsDead())
            {
                GameOver("Kamu mati sebelum misi selesai.");
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator CheckMisi2_BunuhZombie()
    {
        while (true)
        {
            killText.text = $"{zombiesKilled}";
            yield return null;
        }
    }

    IEnumerator CheckMisi3_CariFuel_Tank()
    {
        cratesCollected = 0;

        while (!misi3Selesai)
        {
            if (cratesCollected >= totalCrates)
            {
                misi3Selesai = true;
                HandleMissionComplete(3);
            }

            yield return null;
        }
    }

    void HandleMissionComplete(int missionNum)
    {
        bool wasLast = AllMissionsCompleteAfterThis(missionNum);

        // Tampilkan GOOD Mission X complete jika ini bukan yang terakhir
        if (!wasLast)
        {
            if (missionCompletePopup != null)
            {
                missionCompletePopup.gameObject.SetActive(true);
                missionCompletePopup.text = $"GOOD Mission {missionNum} complete!";
                StartCoroutine(HidePopupAfterSeconds(2f));
            }

            if (audioSource && missionCompleteClip)
                audioSource.PlayOneShot(missionCompleteClip);
        }

        MissionsComplete.occurrence.GetMissionsDone(misi1Selesai, misi2Selesai, misi3Selesai);
        CheckAllMissionDone();
    }

    bool AllMissionsCompleteAfterThis(int currentMission)
    {
        int completed = 0;
        if (misi1Selesai || currentMission == 1) completed++;
        if (misi2Selesai || currentMission == 2) completed++;
        if (misi3Selesai || currentMission == 3) completed++;
        return completed == 3;
    }

    IEnumerator HidePopupAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (missionCompletePopup != null)
        {
            missionCompletePopup.text = "";
            missionCompletePopup.gameObject.SetActive(false);
        }
    }

    void CheckAllMissionDone()
    {
        if (AllMissionsComplete() && !alreadyFinished)
        {
            alreadyFinished = true;

            if (missionCompletePopup != null)
            {
                missionCompletePopup.text = "";
                missionCompletePopup.gameObject.SetActive(false);
            }

            if (allMissionCompleteUI != null)
                allMissionCompleteUI.SetActive(true);

            if (audioSource && allMissionsDoneClip)
                audioSource.PlayOneShot(allMissionsDoneClip);

            StartCoroutine(ShowWinnerPanelCountdown(8));
        }
    }

    IEnumerator ShowWinnerPanelCountdown(int countdown)
    {
        yield return new WaitForSeconds(2f); // waktu nikmati allMissionCompleteUI
        missionCompletePopup.gameObject.SetActive(false);

        if (winnerPanel != null)
            winnerPanel.SetActive(true);

        Time.timeScale = 0f;

        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(() => LoadMainMenu());

        float timeRemaining = countdown;
        while (timeRemaining > 0f)
        {
            if (backToMenuButtonText != null)
                backToMenuButtonText.text = $"Back to Main Menu {Mathf.CeilToInt(timeRemaining)}";

            yield return new WaitForSecondsRealtime(1f);
            timeRemaining--;
        }

        LoadMainMenu();
    }

    void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    bool AllMissionsComplete()
    {
        return misi1Selesai && misi2Selesai && misi3Selesai;
    }

    void GameOver(string reason)
    {
        StopAllCoroutines();
        timerText.text = reason;
    }

    public void RegisterZombieKill()
    {
        zombiesKilled++;
        Debug.Log("Zombie dibunuh: " + zombiesKilled);

        if (!misi2Selesai && zombiesKilled >= zombieTarget)
        {
            misi2Selesai = true;
            HandleMissionComplete(2);
        }
    }

    public void RegisterCrateFound()
    {
        if (!misi3Selesai)
        {
            cratesCollected++;
            Debug.Log("Crate ditemukan! Total: " + cratesCollected);
        }
    }
}
