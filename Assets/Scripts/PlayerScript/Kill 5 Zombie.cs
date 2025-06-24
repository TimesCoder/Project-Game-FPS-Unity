using UnityEngine;
using UnityEngine.UI;

public class ZombieMission : MonoBehaviour
{
    public static ZombieMission Instance;
    public int targetKillCount = 5;
    public float missionDuration = 120f;
    public Text timerText;
    public Text missionStatusText;
    public Text killCountText;

    public AudioSource audioSource;
    public AudioClip timeUpClip;

    private int zombieKilled = 0;
    private bool missionEnded = false;
    private bool timeUpSoundPlayed = false;

    private float gameStartTime;

    void Start()
    {
        gameStartTime = Time.time;

        if (missionStatusText != null)
            missionStatusText.text = "";

        if (killCountText != null)
            killCountText.text = "0";
    }

    void Update()
    {
        float timeSinceStart = Time.time - gameStartTime;

        // ⏱️ Update teks waktu selalu
        timerText.text = "Waktu: " + Mathf.FloorToInt(missionDuration - timeSinceStart) + "s";

        if (missionEnded) return;

        // ✅ Jika zombie cukup dibunuh
        if (zombieKilled >= targetKillCount)
        {
            EndMission(true);
        }

        // ⏰ Jika waktu habis
        else if (timeSinceStart >= missionDuration)
        {
            if (!timeUpSoundPlayed)
            {
                timeUpSoundPlayed = true;

                if (audioSource != null && timeUpClip != null)
                    audioSource.PlayOneShot(timeUpClip);
            }

            EndMission(false);
        }
    }

    public void OnZombieKilled()
    {
        if (missionEnded) return;

        zombieKilled++;
        killCountText.text = zombieKilled.ToString();
        Debug.Log("Zombie dibunuh: " + zombieKilled + "/" + targetKillCount);
    }

    void EndMission(bool success)
    {
        missionEnded = true;

        if (success)
        {
            missionStatusText.text = "✅ Misi Berhasil!";
            missionStatusText.color = Color.green;
            Debug.Log("Misi selesai: BERHASIL");
        }
        else
        {
            missionStatusText.text = "❌ Misi Gagal!";
            missionStatusText.color = Color.red;
            Debug.Log("Misi selesai: GAGAL");
        }

        // Panggil Mission Manager
        if (MissionsComplete.occurrence != null)
            MissionsComplete.occurrence.GetMissionsDone(success, false, false, false);
    }
}
