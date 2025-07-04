// using UnityEngine;

// public class MedkitPickup : MonoBehaviour
// {
//     public int healAmount = 20;
//     private bool isInRange = false;
//     // private PlayerHealth playerHealth;

//     [Header("Sound Effect")]
//     public AudioClip pickupSound;          // Drag audio file ke sini dari Inspector
//     public AudioSource audioSource;

//     void Start()
//     {
//         // Ambil atau tambahkan komponen AudioSource jika belum ada
//         if (audioSource == null)
//         {
//             audioSource = gameObject.AddComponent<AudioSource>();
//         }

//         audioSource.playOnAwake = false;
//     }

//     // void OnTriggerEnter(Collider other)
//     // {
//     //     if (other.CompareTag("Player"))
//     //     {
//     //         isInRange = true;
//     //         playerHealth = other.GetComponent<PlayerHealth>();
//     //     }
//     // }

//     void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             isInRange = false;
//             playerHealth = null;
//         }
//     }

//     void Update()
//     {
//         if (isInRange && Input.GetKeyDown(KeyCode.F))
//         {
//             if (playerHealth != null && !playerHealth.IsHealthFull())
//             {
//                 playerHealth.Heal(healAmount);

//                 // 🔊 Mainkan sound pickup
//                 if (pickupSound != null)
//                 {
//                     audioSource.PlayOneShot(pickupSound);
//                 }

//                 Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
//                 // ⬆ Delay destroy biar sound sempat diputar
//             }
//             else
//             {
//                 Debug.Log("[Medkit] Darah penuh, tidak bisa menggunakan medkit.");
//             }
//         }
//     }
// }
