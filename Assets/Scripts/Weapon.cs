using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Rifle Things")]
    public Camera cam;
    public float giveDamageOf = 10f;
    public float shootingRange = 100f;
    public float fireCharge = 15f;
    private float nextTimeToShoot = 0f;
    public Animator animator;
    public PlayerScript player;
    public Transform hand;

    [Header("Rifle Effects")]
    private int maximumAmunition = 32;
    private int mag = 10;
    private int presentAmunition;
    private float reloadingTime = 1.3f;
    private bool setReloading = false;

    [Header("Rifle Effects")]
    public ParticleSystem muzzleSpark;
    public GameObject WoodedEffect;


    private void Awake()
    {
        transform.SetParent(hand);
        presentAmunition = maximumAmunition;
    }
    private void Update()
    {
        if (setReloading)
            return;

        if (presentAmunition <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToShoot)
        {
            animator.SetBool("fire", true);
            animator.SetBool("idle", false);
            nextTimeToShoot = Time.time + 1f / fireCharge;
            Shoot();
        }
        else if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("idle", false);
            animator.SetBool("firewalk", true);
        }
        else if (Input.GetButton("Fire2") && Input.GetButton("Fire1"))
        {
            animator.SetBool("idle", false);
            animator.SetBool("idleaim", true);
            animator.SetBool("firewalk", true);
            animator.SetBool("walk", true);
            animator.SetBool("reloading", false);
        }
        else
        {
            animator.SetBool("fire", false);
            animator.SetBool("idle", true);
            animator.SetBool("firewalk", false);
        }
    }

    private void Shoot()
    {
        if (mag == 0)
        {
            return;
        }

        presentAmunition--;

        if (presentAmunition == 0)
        {
            mag--;
        }


        muzzleSpark.Play();
        RaycastHit hitInfo;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
        {
            Debug.Log(hitInfo.transform.name);

            ObjectToHit objectToHit = hitInfo.transform.GetComponent<ObjectToHit>();

            if (objectToHit != null)
            {
                objectToHit.ObjectHitDamage(giveDamageOf);
                GameObject WoodGo = Instantiate(WoodedEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(WoodGo, 1f);
            }
        }
    }

    IEnumerator Reload()
    {
        player.walkSpeed = 0f;
        player.sprintSpeed = 0f;
        setReloading = true;
        Debug.Log("Reloading....");
        animator.SetBool("reloading", true);
        yield return new WaitForSeconds(reloadingTime);
        presentAmunition = maximumAmunition;
        player.walkSpeed = 1.9f;
        player.sprintSpeed = 3f;
        setReloading = false;
        animator.SetBool("reloading", false);
    }
}
