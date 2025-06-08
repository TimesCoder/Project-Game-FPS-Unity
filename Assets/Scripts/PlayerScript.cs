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

    // Reload handling
    public float reloadDuration = 2f;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    void Start()
    {
        fireLayerIndex = animator.GetLayerIndex("Fire");
        weaponLayerIndex = animator.GetLayerIndex("Weapon");
        UpdateWeaponState();
    }

    void Update()
    {
        UpdateMuzzleFlashReference();

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                ReloadComplete();
            }
            return; // Tidak bisa menembak saat reload
        }

        WeaponInstance currentWeapon = playerInventory.GetCurrentWeaponInstance();
        if (currentWeapon == null) return;

        if (Input.GetMouseButton(0))
        {
            if (currentWeapon.currentAmmo > 0)
            {
                animator.SetBool("Fire", true);
                animator.SetLayerWeight(fireLayerIndex, 1f);
                Fire();
            }
            else
            {
                StartReload();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("Fire", false);
            animator.SetLayerWeight(fireLayerIndex, 0f);
            if (muzzleFlash != null && muzzleFlash.isPlaying)
                muzzleFlash.Stop();
        }

        // Reload manual dengan tombol R
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentWeapon.currentAmmo < currentWeapon.weaponData.maxAmmo)
        {
            StartReload();
        }
    }

    void UpdateMuzzleFlashReference()
    {
        if (playerInventory.weaponHolder.childCount > 0)
        {
            var currentWeaponGO = playerInventory.weaponHolder.GetChild(0).gameObject;
            if (muzzleFlash == null || muzzleFlash.gameObject != currentWeaponGO)
            {
                muzzleFlash = currentWeaponGO.GetComponentInChildren<ParticleSystem>();
                if (muzzleFlash != null)
                    muzzleFlash.Stop();
            }
        }
        else
        {
            muzzleFlash = null;  // reset saat senjata hilang
        }
    }


    void Fire()
    {
        WeaponInstance currentWeapon = playerInventory.GetCurrentWeaponInstance();
        if (currentWeapon == null || currentWeapon.currentAmmo <= 0)
            return;

        currentWeapon.currentAmmo--;

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
                int damage = currentWeapon.weaponData.damage;
                Debug.Log("Dealing " + damage + " damage.");

                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }

        // Jika ammo habis setelah tembakan
        if (currentWeapon.currentAmmo <= 0)
        {
            StartReload();
        }
    }

    void StartReload()
    {
        if (isReloading) return;

        WeaponInstance currentWeapon = playerInventory.GetCurrentWeaponInstance();
        if (currentWeapon == null || currentWeapon.currentAmmo == currentWeapon.weaponData.maxAmmo)
            return;

        isReloading = true;
        reloadTimer = reloadDuration;

        animator.SetBool("Reload", true);
        animator.SetBool("Fire", false);

        if (muzzleFlash != null && muzzleFlash.isPlaying)
            muzzleFlash.Stop();

        Debug.Log("Reload started for weapon: " + currentWeapon.weaponData.weaponName);
    }

    void ReloadComplete()
    {
        WeaponInstance currentWeapon = playerInventory.GetCurrentWeaponInstance();
        if (currentWeapon != null)
            currentWeapon.currentAmmo = currentWeapon.weaponData.maxAmmo;

        isReloading = false;
        animator.SetBool("Reload", false);

        Debug.Log("Reload complete for weapon: " + currentWeapon.weaponData.weaponName);
    }

    public void UpdateWeaponState()
    {
        bool hasWeapon = playerInventory.GetCurrentWeaponInstance() != null;

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
