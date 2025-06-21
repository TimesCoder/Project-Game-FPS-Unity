using UnityEngine;
using UnityEngine.AI;

public class ZombieBiasa : EnemyHealth
{
    public float patrolSpeed = 0.5f;
    public float chaseSpeed = 3.5f;
    public float detectionRadius = 15f;
    public float attackRange = 1f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;
    public float giveUpChaseDistance = 25f;
    public float chaseMemoryTime = 5f;

    [Header("Audio")]
    public AudioClip zombieSound;
    public AudioClip attackSound;
    public AudioSource audioSource;

    private Transform player;
    private PlayerHealth playerHealth;
    private float attackTimer;
    private float timeSinceLastSawPlayer = 0f;
    private Vector3 patrolTarget;
    private bool isChasing = false;
    private float zombieSoundTimer = 0f;
    private float zombieSoundInterval = 5f; // Zombie growl setiap 5 detik

    protected override void Start()
    {
        maxHealth = 200;
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            enabled = false;
            return;
        }

        playerHealth = player.GetComponent<PlayerHealth>();
        SetRandomPatrolTarget();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (isDead || player == null) return;

        attackTimer += Time.deltaTime;
        zombieSoundTimer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            timeSinceLastSawPlayer = 0f;
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
            timeSinceLastSawPlayer = 0f;
            ChasePlayer();
        }
        else if (isChasing)
        {
            timeSinceLastSawPlayer += Time.deltaTime;

            if (timeSinceLastSawPlayer <= chaseMemoryTime && distanceToPlayer <= giveUpChaseDistance)
            {
                ChasePlayer();
            }
            else
            {
                isChasing = false;
                Patrol();
            }
        }
        else
        {
            Patrol();
        }

        // Putar suara growl zombie secara berkala
        if (zombieSound != null && zombieSoundTimer >= zombieSoundInterval)
        {
            audioSource.PlayOneShot(zombieSound);
            zombieSoundTimer = 0f;
        }
    }

    void Patrol()
    {
        if (!agent.enabled) return;

        agent.speed = patrolSpeed;
        animator.SetBool("walk", true);
        animator.SetBool("run", false);
        animator.SetBool("attack", false);

        if (playerHealth != null) playerHealth.DisableHitEffect();

        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            SetRandomPatrolTarget();
        }

        agent.SetDestination(patrolTarget);
    }

    void ChasePlayer()
    {
        if (!agent.enabled) return;

        agent.speed = chaseSpeed;
        animator.SetBool("walk", false);
        animator.SetBool("run", true);
        animator.SetBool("attack", false);

        if (playerHealth != null) playerHealth.DisableHitEffect();

        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        if (!agent.enabled) return;

        agent.SetDestination(transform.position);
        animator.SetBool("walk", false);
        animator.SetBool("run", false);
        animator.SetBool("attack", true);

        if (playerHealth != null) playerHealth.EnableHitEffect();

        if (attackTimer >= attackCooldown)
        {
            Debug.Log($"{gameObject.name} menyerang player! Damage: {attackDamage}");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            // 🔊 Suara attack
            if (attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            attackTimer = 0f;
        }
    }

    void SetRandomPatrolTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * 10;
        randomDir += transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
        }
    }
}
