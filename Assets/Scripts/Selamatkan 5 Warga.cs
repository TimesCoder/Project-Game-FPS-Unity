using UnityEngine;
using UnityEngine.UI;

public class CitizenRescueMission : MonoBehaviour
{
    public int targetCitizenCount = 5;
    public Text missionStatusText;

    private int citizenSaved = 0;
    private bool missionEnded = false;

    void Start()
    {
        missionStatusText.text = "";
    }

    // Dipanggil saat warga berhasil diselamatkan
    public void OnCitizenSaved()
    {
        if (missionEnded) return;

        citizenSaved++;
        Debug.Log("Warga diselamatkan: " + citizenSaved + "/" + targetCitizenCount);

        if (citizenSaved >= targetCitizenCount)
        {
            EndMission(true);
        }
    }

    void EndMission(bool success)
    {
        missionEnded = true;

        if (success)
        {
            missionStatusText.text = "Misi 2 Berhasil!";
            Debug.Log("Misi penyelamatan selesai. Status: Berhasil");

            // Tandai misi 2 selesai
            MissionsComplete.occurrence.GetMissionsDone(false, true, false, false);
        }
        else
        {
            missionStatusText.text = "Misi 2 Gagal!";
            Debug.Log("Misi penyelamatan selesai. Status: Gagal");
        }
    }
}
