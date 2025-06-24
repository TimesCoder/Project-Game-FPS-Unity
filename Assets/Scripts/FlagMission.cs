using UnityEngine;
using UnityEngine.UI;

public class FlagMission : MonoBehaviour
{
    public int targetFlagCount = 3;
    public Text missionStatusText;
    public Text flagCountText;

    private int foundFlags = 0;
    private bool missionEnded = false;

    void Start()
    {
        missionStatusText.text = "";
        flagCountText.text = "0";
    }

    public void OnFlagFound()
    {
        if (missionEnded) return;

        foundFlags++;
        flagCountText.text = foundFlags.ToString();
        Debug.Log("Bendera ditemukan: " + foundFlags + "/" + targetFlagCount);

        if (foundFlags >= targetFlagCount)
        {
            CompleteMission();
        }
    }

    void CompleteMission()
    {
        missionEnded = true;
        missionStatusText.text = "✅ Misi Berhasil!";
        missionStatusText.color = Color.green;

        if (MissionsComplete.occurrence != null)
            MissionsComplete.occurrence.GetMissionsDone(false, false, true, false);
    }
}
