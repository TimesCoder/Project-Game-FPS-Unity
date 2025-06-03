using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [Header("Weapon`s")]
    public GameObject PlayerWeapon;
    public GameObject PickupWeapon;
    public PlayerPunch playerPunch;

    [Header("Weapon Assign Things")]
    public GameObject player;
    public float radius = 2.5f;
    public Animator animator;
    public float nextTimeToPunch = 0f;
    public float punchCharge = 15f;

    private void Awake()
    {
        PlayerWeapon.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToPunch)
        {
            animator.SetBool("punch", true);
            animator.SetBool("idle", false);
            nextTimeToPunch = Time.time + 1f / punchCharge;
            playerPunch.Punch();
        }else
        {
            animator.SetBool("punch", false);
            animator.SetBool("idle", true);
        }
        if (Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (Input.GetKeyDown("f"))
            {
                PlayerWeapon.SetActive(true);
                PickupWeapon.SetActive(false);
            }
        }
    }


}
