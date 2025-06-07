using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public Transform rayCastFirePoint;
    public float rayDistance = 1000f;
    public Animator animator;
    public PlayerInventory playerInventory;

    private int fireLayerIndex;
    private int weaponLayerIndex;

    private ParticleSystem muzzleFlash;  // Ini diambil dari prefab senjata aktif

    public GameObject HitEffectPrefab;   // Efek hit point (impact)

    void Start()
    {
        fireLayerIndex = animator.GetLayerIndex("Fire");
        weaponLayerIndex = animator.GetLayerIndex("Weapon");
        UpdateWeaponState();
    }

    void Update()
    {
        UpdateMuzzleFlashReference();

        if (Input.GetMouseButton(0))  
        {
            if (playerInventory.weapons.Count > 0)
            {
                animator.SetBool("Fire", true);
                animator.SetLayerWeight(fireLayerIndex, 1f);
                Fire();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("Fire", false);
            animator.SetLayerWeight(fireLayerIndex, 0f);
            if (muzzleFlash != null && muzzleFlash.isPlaying)
                muzzleFlash.Stop();
        }
    }


    void UpdateMuzzleFlashReference()
    {
        if (playerInventory.weaponHolder.childCount > 0)
        {
            var currentWeapon = playerInventory.weaponHolder.GetChild(0).gameObject;
            if (muzzleFlash == null || muzzleFlash.gameObject != currentWeapon)
            {
                muzzleFlash = currentWeapon.GetComponentInChildren<ParticleSystem>();
                if (muzzleFlash != null)
                    muzzleFlash.Stop(); // Pastikan efek off dulu
            }
        }
    }

    void Fire()
    {
        if (playerInventory.weapons.Count == 0) return;

        // Play efek muzzle flash yang sudah ada di prefab
        if (muzzleFlash != null)
            muzzleFlash.Play();

        Ray ray = new Ray(rayCastFirePoint.position, rayCastFirePoint.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 2f);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);

            if (HitEffectPrefab != null)
            {
                GameObject impact = Instantiate(HitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }

            if (hit.collider.CompareTag("Enemy"))
            {
                int damage = playerInventory.GetCurrentWeaponDamage();
                Debug.Log("Dealing " + damage + " damage.");

                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }
    }

    public void UpdateWeaponState()
    {
        bool hasWeapon = playerInventory.weapons.Count > 0;

        animator.SetLayerWeight(weaponLayerIndex, hasWeapon ? 1f : 0f);

        if (!hasWeapon)
        {
            animator.SetBool("Fire", false);
            animator.SetLayerWeight(fireLayerIndex, 0f);
            if (muzzleFlash != null)
                muzzleFlash.Stop();
        }
    }
}
