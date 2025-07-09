using UnityEngine;

public class CratePickup : MonoBehaviour
{
    public float pickupRadius = 3f;
    public AudioClip pickupSound;

    private bool picked = false;
    private Transform player;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (picked || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= pickupRadius && Input.GetKeyDown(KeyCode.F))
        {
            picked = true;

            // Register mission progress
            if (ZombieMissionManager.Instance != null)
                ZombieMissionManager.Instance.RegisterCrateFound();

            // Play sound if assigned
            if (pickupSound != null)
                audioSource.PlayOneShot(pickupSound);

            // Hapus crate setelah durasi sound
            Invoke(nameof(DestroySelf), pickupSound != null ? pickupSound.length : 0f);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
