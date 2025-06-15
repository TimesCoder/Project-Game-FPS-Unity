using UnityEngine;
using UnityEngine.UI;

public class ZombieMission : MonoBehaviour
{
    public int targetKillCount = 20;
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
        missionStatusText.text = "";
        killCountText.text = "x 0";
    }

    void Update()
    {
        float timeSinceStart = Time.time - gameStartTime;

        // ⏱️ Update timerText selalu, meskipun misi sudah berakhir
        timerText.text = "Waktu: " + Mathf.FloorToInt(timeSinceStart) + "s";

        // 🚫 Jika misi sudah berakhir, tidak perlu proses logika di bawah
        if (missionEnded) return;

        // ⏰ Cek apakah durasi misi habis
        if (timeSinceStart >= missionDuration)
        {
            if (!timeUpSoundPlayed)
            {
                timeUpSoundPlayed = true;

                if (audioSource != null && timeUpClip != null)
                {
                    audioSource.PlayOneShot(timeUpClip);
                }

                missionStatusText.text = "Waktu Habis!";
            }

            EndMission(false);
            return;
        }

        // ✅ Cek apakah zombie sudah cukup dibunuh
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
            MissionsComplete.occurrence.GetMissionsDone(true, false, false, false);
        }
        else
        {
            missionStatusText.text = "Misi Gagal!";
            Debug.Log("Misi selesai. Status: Gagal");

            // Mainkan suara jika belum dimainkan
            if (!timeUpSoundPlayed && audioSource != null && timeUpClip != null)
            {
                audioSource.PlayOneShot(timeUpClip);
                timeUpSoundPlayed = true;
            }
        }
    }
}
