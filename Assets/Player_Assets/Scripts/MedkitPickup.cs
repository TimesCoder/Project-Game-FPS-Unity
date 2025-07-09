using UnityEngine;

public class MedkitPickup : MonoBehaviour
{
    [Header("Healing")]
    public float healAmount = 25f;
    public float pickupRadius = 3f;

    [Header("Sound")]
    public AudioClip pickupSound;
    private AudioSource audioSource;

    private bool pickedUp = false;
    private Transform player;

    void Start()
    {
        // Cari player berdasarkan tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Tambahkan AudioSource jika belum ada
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (pickedUp || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= pickupRadius && Input.GetKeyDown(KeyCode.F))
        {
            HealthController playerHealth = player.GetComponent<HealthController>();
            if (playerHealth != null && !playerHealth.IsDead())
            {
                pickedUp = true;

                // Tambahkan HP ke player
                playerHealth.Heal(healAmount); // ⬅️ fungsi ini akan kita buat di HealthController

                // Mainkan suara pickup
                if (pickupSound != null)
                    audioSource.PlayOneShot(pickupSound);

                // Hapus medkit setelah suara selesai
                Invoke(nameof(DestroySelf), pickupSound != null ? pickupSound.length : 0f);
            }
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    // Tambahkan radius visual di editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
