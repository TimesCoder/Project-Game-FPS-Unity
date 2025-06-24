// using UnityEngine;
// using UnityEngine.UI;

// public class SurviveMission : MonoBehaviour
// {
//     public ZombieMission zombieMission;
//     public CivilianMission civilianMission;
//     public FlagMission flagMission;

//     public Text missionStatusText;

//     private bool missionCompleted = false;

//     void Start()
//     {
//         missionStatusText.text = "";
//     }

//     void Update()
//     {
//         if (missionCompleted) return;

//         if (IsAllPreviousMissionsComplete())
//         {
//             CompleteMission();
//         }
//     }

//     bool IsAllPreviousMissionsComplete()
//     {
//         return zombieMission != null && civilianMission != null && flagMission != null
//             && zombieMission.IsMissionSuccessful()
//             && civilianMission.IsMissionSuccessful()
//             && flagMission.IsMissionSuccessful();
//     }

//     void CompleteMission()
//     {
//         missionCompleted = true;
//         missionStatusText.text = "✅ Misi Selesai!";
//         missionStatusText.color = Color.green;

//         if (MissionsComplete.occurrence != null)
//             MissionsComplete.occurrence.GetMissionsDone(false, false, false, true);

//         Debug.Log("Misi ke-4 (Survive) BERHASIL karena semua misi sebelumnya selesai.");
//     }
// }
