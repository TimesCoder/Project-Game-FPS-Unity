using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public PlayMakerFSM movementFSM;
    public Transform rayCastFirePoint;
    public float rayDistance = 1000f;
    public Animator animator;
    public PlayerInventory playerInventory;
    public GameObject HitEffectPrefab;
    public float reloadDuration = 3f;

    private int fireLayerIndex;
    private int weaponLayerIndex;

    private ParticleSystem muzzleFlash;
    private bool isReloading = false;
    private float reloadTimer = 0f;
    public GameObject resumeMenuPanel;

    public AudioClip shootingSound;
    public AudioClip reloadingSound;
    public AudioSource audioSource;



    void Start()
    {
        fireLayerIndex = animator.GetLayerIndex("Fire");
        weaponLayerIndex = animator.GetLayerIndex("Weapon");
        UpdateWeaponState();
    }

    void Update()
    {
        // Jika menu pause aktif, hentikan semua aktivitas player fire
        if (resumeMenuPanel != null && resumeMenuPanel.activeInHierarchy)
        {
            animator.SetBool("Fire", false);
            animator.SetBool("Reload", false);
            StopFiringAnimation();
            return;
        }

        UpdateMuzzleFlashReference();

        if (isReloading)
        {
            HandleReloading();
            return;
        }

        WeaponInstance currentWeapon = playerInventory.GetCurrentWeaponInstance();
        if (currentWeapon == null) return;

        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.currentAmmo < currentWeapon.weaponData.maxAmmo)
        {
            StartReload();
            return;
        }

        HandleFiring(currentWeapon);
    }


    void HandleFiring(WeaponInstance currentWeapon)
    {
        if (Input.GetMouseButton(0))
        {
            if (currentWeapon.currentAmmo > 0)
            {
                animator.SetBool("Fire", true);
                animator.SetLayerWeight(fireLayerIndex, 1f);
                Fire(currentWeapon);
            }
            else
            {
                StartReload(); // Auto reload
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopFiringAnimation();
        }
    }

    void Fire(WeaponInstance currentWeapon)
    {
        currentWeapon.currentAmmo--;

        if (muzzleFlash != null)
            muzzleFlash.Play();
        audioSource.PlayOneShot(shootingSound);

        Ray ray = new Ray(rayCastFirePoint.position, rayCastFirePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 2f);
            Debug.Log("Raycast hit: " + hit.collider.name);

            if (HitEffectPrefab != null)
            {
                GameObject impact = Instantiate(HitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f);
            }

            if (hit.collider != null)
            {
                string hitTag = hit.collider.tag;

                if (hitTag == "Head" || hitTag == "Body")
                {
                    int damage = currentWeapon.weaponData.damage;

                    EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(damage, hitTag);
                    }
                }
            }

        }

        if (currentWeapon.currentAmmo <= 0)
        {
            StartReload(); // Auto reload
        }
    }

    void StartReload()
    {
        if (isReloading) return;

        WeaponInstance currentWeapon = playerInventory.GetCurrentWeaponInstance();
        if (currentWeapon == null || currentWeapon.currentAmmo >= currentWeapon.weaponData.maxAmmo)
            return;

        isReloading = true;
        reloadTimer = reloadDuration;

        StopFiringAnimation();

        animator.SetBool("Reload", true);
        audioSource.PlayOneShot(reloadingSound);
        animator.SetLayerWeight(fireLayerIndex, 1f);

        Debug.Log("Reload started for weapon: " + currentWeapon.weaponData.weaponName);

        if (muzzleFlash != null)
            muzzleFlash.Stop();
    }

    void HandleReloading()
    {
        reloadTimer -= Time.deltaTime;

        if (reloadTimer <= 0f)
        {
            ReloadComplete();
        }
    }

    void ReloadComplete()
    {
        WeaponInstance currentWeapon = playerInventory.GetCurrentWeaponInstance();
        if (currentWeapon != null)
        {
            currentWeapon.currentAmmo = currentWeapon.weaponData.maxAmmo;
            Debug.Log("Reload complete for weapon: " + currentWeapon.weaponData.weaponName);
        }

        isReloading = false;
        animator.SetBool("Reload", false);
        animator.SetLayerWeight(fireLayerIndex, 0f);
    }

    void StopFiringAnimation()
    {
        animator.SetBool("Fire", false);
        animator.SetLayerWeight(fireLayerIndex, 0f);

        if (muzzleFlash != null && muzzleFlash.isPlaying)
            muzzleFlash.Stop();
    }

    void UpdateMuzzleFlashReference()
    {
        if (playerInventory.weaponHolder.childCount > 0)
        {
            GameObject currentWeaponGO = playerInventory.weaponHolder.GetChild(0).gameObject;

            if (muzzleFlash == null || muzzleFlash.gameObject != currentWeaponGO)
            {
                muzzleFlash = currentWeaponGO.GetComponentInChildren<ParticleSystem>();
                muzzleFlash?.Stop();
            }
        }
        else
        {
            muzzleFlash = null;
        }
    }

    public void UpdateWeaponState()
    {
        bool hasWeapon = playerInventory.GetCurrentWeaponInstance() != null;

        animator.SetLayerWeight(weaponLayerIndex, hasWeapon ? 1f : 0f);
        animator.SetLayerWeight(fireLayerIndex, 0f);

        if (movementFSM != null)
        {
            var hasWeaponVar = movementFSM.FsmVariables.GetFsmBool("HasWeapon");
            if (hasWeaponVar != null)
                hasWeaponVar.Value = hasWeapon;

            movementFSM.SendEvent("CheckWeaponAgain");
        }

        if (!hasWeapon)
        {
            StopFiringAnimation();
            animator.SetBool("Reload", false);
            animator.SetLayerWeight(fireLayerIndex, 0f);

            if (muzzleFlash != null)
                muzzleFlash.Stop();
        }
    }
}
