using UnityEngine;

public class CratePickup : MonoBehaviour
{
    public float pickupRadius = 3f;
    private bool picked = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (picked || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= pickupRadius && Input.GetKeyDown(KeyCode.F))
        {
            picked = true;
            ZombieMissionManager.Instance.RegisterCrateFound();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
