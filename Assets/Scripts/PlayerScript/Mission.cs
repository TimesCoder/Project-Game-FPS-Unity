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
    public Text mission4;

    public static MissionsComplete occurrence;
    
    private void Awake()
    {
        occurrence = this;
    }

    public void GetMissionsDone(bool misi1, bool misi2, bool misi3, bool misi4)
    {
        if (misi1 == true)
        {
            mission1.text = "mission 1: Completed";
            mission1.color = Color.green;
        }
        else
        {
            mission1.text = "Bunuh 20 Zombie Dalam Waktu 2 Menit";
            mission1.color = Color.white;
        }

        if (misi2 == true)
        {
            mission2.text = "mission 2: Completed";
            mission2.color = Color.green;
        }
        else
        {
            mission2.text = "Selamatkan 5 Warga Dari Seragan Zombie";
            mission2.color = Color.white;
        }

        if (misi3 == true)
        {
            mission3.text = "mission 3: Completed";
            mission3.color = Color.green;
        }
        else
        {
            mission3.text = "Bertahan Dari Serangan Zombie Selama 1 Menit Tanpa Senjata";
            mission3.color = Color.white;
        }

        if (misi4 == true)
        {
            mission4.text = "mission 4: Completed";
            mission4.color = Color.green;
        }
        else
        {
            mission4.text = "mission 4: Not Completed";
            mission4.color = Color.red;
        }
    }
}    


