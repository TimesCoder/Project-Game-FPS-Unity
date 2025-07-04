using UnityEngine;
using scgFullBodyController;


public class WeaponPickup : MonoBehaviour
{
    public int weaponSlotIndex;
    public float pickupRadius = 3f;

    private GameObject player;
    private GunManager gunManager;
    private bool pickedUp = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            gunManager = player.GetComponent<GunManager>();
    }

    void Update()
    {
        if (pickedUp || gunManager == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= pickupRadius)
        {
            // Player menekan F dan belum punya senjata ini
            if (Input.GetKeyDown(KeyCode.F) && !gunManager.IsWeaponUnlocked(weaponSlotIndex))
            {
                gunManager.UnlockWeapon(weaponSlotIndex);
                pickedUp = true;
                Destroy(gameObject); // hapus senjatanya dari tanah
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
