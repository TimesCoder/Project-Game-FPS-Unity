using UnityEngine;
using UnityEngine.UI;

public class UnarmedSurvivalMission : MonoBehaviour
{
    public float survivalTime = 60f;
    public Text missionStatusText;
    public bool isPlayerDead = false;

    private float timeRemaining;
    private bool missionStarted = false;
    private bool missionEnded = false;
    private bool playerHasWeapon = false;

    void Start()
    {
        missionStatusText.text = "BERTAHAN DARI SERANGAN ZOMBIE SELAMA 1 MENIT TANPA SENJATA";
        timeRemaining = survivalTime;
        missionStarted = true;
    }

    void Update()
    {
        if (!missionStarted || missionEnded) return;

        // Cek jika pemain punya senjata atau mati
        if (playerHasWeapon || isPlayerDead)
        {
            EndMission(false);
            return;
        }

        // Kurangi waktu
        timeRemaining -= Time.deltaTime;

        // Update teks UI (opsional)
        missionStatusText.text = "BERTAHAN: " + Mathf.CeilToInt(timeRemaining) + " DETIK";

        if (timeRemaining <= 0)
        {
            EndMission(true);
        }
    }

    public void SetPlayerDead()
    {
        isPlayerDead = true;
    }

    public void SetPlayerHasWeapon(bool hasWeapon)
    {
        playerHasWeapon = hasWeapon;
    }

    private void EndMission(bool success)
    {
        missionEnded = true;

        if (success)
        {
            missionStatusText.text = "MISI 3 BERHASIL!";
            Debug.Log("Misi bertahan hidup selesai: BERHASIL");
            MissionsComplete.occurrence.GetMissionsDone(false, false, true, false);
        }
        else
        {
            missionStatusText.text = "MISI 3 GAGAL!";
            Debug.Log("Misi bertahan hidup selesai: GAGAL");
        }
    }
}
