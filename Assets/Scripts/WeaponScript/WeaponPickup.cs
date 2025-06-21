using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapon weaponData;
    public float pickupRadius = 1.5f;

    [Header("Sound Effect")]
    public AudioClip pickupSound;
    public AudioSource audioSource;

    private GameObject player;
    private PlayerInventory playerInventory;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
        }

        // Tambahkan AudioSource jika belum ada
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (player == null || playerInventory == null) return;

        if (Vector3.Distance(transform.position, player.transform.position) < pickupRadius)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (playerInventory.AddWeapon(weaponData))
                {
                    // Mainkan suara pickup
                    if (pickupSound != null)
                    {
                        audioSource.PlayOneShot(pickupSound);
                    }

                    // Hancurkan object setelah durasi suara
                    Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
