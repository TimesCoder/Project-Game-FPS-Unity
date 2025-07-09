using UnityEngine;
using scgFullBodyController;

public class WeaponPickup : MonoBehaviour
{
    public int weaponSlotIndex;
    public float pickupRadius = 3f;
    public AudioClip pickupSound;

    private GameObject player;
    private GunManager gunManager;
    private bool pickedUp = false;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            gunManager = player.GetComponent<GunManager>();

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (pickedUp || gunManager == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= pickupRadius && Input.GetKeyDown(KeyCode.F))
        {
            if (!gunManager.IsWeaponUnlocked(weaponSlotIndex))
            {
                gunManager.UnlockWeapon(weaponSlotIndex);
                pickedUp = true;

                // Play pickup sound
                if (pickupSound != null)
                    audioSource.PlayOneShot(pickupSound);

                // Delay destroy so sound can finish
                Invoke(nameof(DestroySelf), pickupSound != null ? pickupSound.length : 0f);
            }
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
