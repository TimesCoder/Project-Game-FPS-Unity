using UnityEngine;
using UnityEngine.UI;

public class ZombieMission : MonoBehaviour
{
    public int targetKillCount = 20;
    public float timeLimit = 120f; // 2 menit
    public Text timerText;
    public Text missionStatusText;
    public Text killCountText; // ✅ Ini dipakai di OnZombieKilled

    private float timeRemaining;
    private int zombieKilled = 0;
    private bool missionEnded = false;

    void Start()
    {
        timeRemaining = timeLimit;
        missionStatusText.text = "";
        killCountText.text = "x 0"; // Awal tampilan kill
    }

    void Update()
    {
        if (missionEnded) return;

        // Hitung mundur waktu
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Waktu: " + Mathf.CeilToInt(timeRemaining) + "s";
        }
        else
        {
            timeRemaining = 0;
            EndMission(false);
        }

        // Cek jika zombie yang dibunuh sudah cukup
        if (zombieKilled >= targetKillCount)
        {
            EndMission(true);
        }
    }

    public void OnZombieKilled()
    {
        if (missionEnded) return;

        zombieKilled++;

        killCountText.text = "x " + zombieKilled;

        Debug.Log("Zombie dibunuh: " + zombieKilled + "/" + targetKillCount);
    }

    void EndMission(bool success)
    {
        missionEnded = true;

        if (success)
        {
            missionStatusText.text = "Misi Berhasil!";
            Debug.Log("Misi selesai. Status: Berhasil");

            // Tandai misi 1 selesai (misi lain belum)
            MissionsComplete.occurrence.GetMissionsDone(true, false, false, false);
        }
        else
        {
            missionStatusText.text = "Misi Gagal!";
            Debug.Log("Misi selesai. Status: Gagal");
        }
    }
}
