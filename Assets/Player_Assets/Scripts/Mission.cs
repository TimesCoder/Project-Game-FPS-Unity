using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsComplete : MonoBehaviour
{
    [Header("Missions to Complete")]
    public Text mission1;
    public Text mission2;
    public Text mission3;
    public static MissionsComplete occurrence;

    private void Awake()
    {
        occurrence = this;
    }

    public void GetMissionsDone(bool misi1, bool misi2, bool misi3)
    {
        mission1.text = misi1 ? "Misi 1: Completed" : "Bertahan Hidup Selama 1 Menit";
        mission1.color = misi1 ? Color.green : Color.white;

        mission2.text = misi2 ? "Misi 2: Completed" : "Bunuh 20 Zombie";
        mission2.color = misi2 ? Color.green : Color.white;

        mission3.text = misi3 ? "Misi 3: Completed" : "Temukan 3 Fuel Tank";
        mission3.color = misi3 ? Color.green : Color.white;
    }
}
