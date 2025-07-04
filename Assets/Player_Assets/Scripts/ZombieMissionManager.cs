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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (allMissionCompleteUI != null) allMissionCompleteUI.SetActive(false);
        if (missionCompletePopup != null) missionCompletePopup.text = "";

        MissionsComplete.occurrence.GetMissionsDone(false, false, false);

        waktuMisi1Mulai = Time.time;

        StartCoroutine(CheckMisi1_Bertahan());
        StartCoroutine(CheckMisi2_BunuhZombie());
        StartCoroutine(CheckMisi3_CariFuel_Tank());
    }

    // Misi 1: Bertahan Hidup selama 60 detik
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

                if (timerText != null) timerText.gameObject.SetActive(false);

                MissionComplete(1);
                CheckAllMissionDone();
            }


            if (HealthController_Player.Instance.IsDead())
            {
                GameOver("Kamu mati sebelum misi selesai.");
                yield break;
            }

            yield return null;
        }
    }

    // Misi 2: Bunuh Zombie (tanpa batas waktu)
    IEnumerator CheckMisi2_BunuhZombie()
    {
        zombiesKilled = 0;

        while (!misi2Selesai)
        {
            killText.text = $"{zombiesKilled}";

            if (zombiesKilled >= zombieTarget)
            {
                misi2Selesai = true;
                MissionComplete(2);
                CheckAllMissionDone();
            }

            yield return null;
        }
    }

    // Misi 3: Cari Fuel_Tank (tanpa batas waktu)
    IEnumerator CheckMisi3_CariFuel_Tank()
    {
        cratesCollected = 0;

        while (!misi3Selesai)
        {
            // crateText.text = $"Fuel_Tank: {cratesCollected}/{totalCrates}";
            // Debug.Log($"Cek Fuel_Tank: {cratesCollected}/{totalCrates}");

            if (cratesCollected >= totalCrates)
            {
                Debug.Log("Misi 3 selesai!");
                misi3Selesai = true;
                MissionComplete(3);
                CheckAllMissionDone();
            }

            yield return null;
        }
    }


    void MissionComplete(int missionNum)
    {
        if (missionCompletePopup != null)
        {
            missionCompletePopup.text = $"Misi {missionNum} Selesai!";
            StartCoroutine(HidePopupAfterSeconds(2f));
        }

        if (audioSource && missionCompleteClip)
            audioSource.PlayOneShot(missionCompleteClip);

        MissionsComplete.occurrence.GetMissionsDone(misi1Selesai, misi2Selesai, misi3Selesai);
    }

    IEnumerator HidePopupAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (missionCompletePopup != null)
            missionCompletePopup.text = "";
    }

    void CheckAllMissionDone()
    {
        if (misi1Selesai && misi2Selesai && misi3Selesai)
        {
            if (missionCompletePopup != null)
                missionCompletePopup.text = "ALL MISSIONS COMPLETED!";

            if (audioSource && allMissionsDoneClip)
                audioSource.PlayOneShot(allMissionsDoneClip);

            if (allMissionCompleteUI)
                allMissionCompleteUI.SetActive(true);

            StartCoroutine(DelayThenLoadNextScene());
        }
    }

    IEnumerator DelayThenLoadNextScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Main Menu");
    }

    void GameOver(string reason)
    {
        StopAllCoroutines();
        timerText.text = reason;
    }

    public void RegisterZombieKill()
    {
        if (!misi2Selesai)
            zombiesKilled++;
    }

    public void RegisterCrateFound()
    {
        if (!misi3Selesai)
        {
            cratesCollected++;
            Debug.Log("Crate ditemukan! Total: " + cratesCollected);

            if (cratesCollected >= totalCrates)
            {
                Debug.Log("Semua crate ditemukan! Menyelesaikan misi...");
            }
        }
    }


}
