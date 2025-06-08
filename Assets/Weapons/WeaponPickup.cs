using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapon weaponData;
    public float pickupRadius = 1.5f;
    
    private GameObject player;
    private PlayerInventory playerInventory;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerInventory = player.GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (player == null) return;
        
        if (Vector3.Distance(transform.position, player.transform.position) < pickupRadius)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (playerInventory.AddWeapon(weaponData))
                {
                    Destroy(gameObject);
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