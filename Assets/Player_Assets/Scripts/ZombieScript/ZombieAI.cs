using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("Zombie Settings")]
    public float patrolSpeed = 0.5f;
    public float chaseSpeed = 3.5f;
    public float detectionRadius = 15f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;
    public float giveUpChaseDistance = 25f;
    public float chaseMemoryTime = 5f;

    [Header("Audio")]
    public AudioClip zombieSound;
    public AudioClip attackSound;
    public AudioSource audioSource;

    [Header("Animation")]
    public Animator animator;

    private Transform player;
    private HealthController playerHealth;
    private NavMeshAgent agent;
    private float attackTimer;
    private float timeSinceLastSawPlayer;
    private Vector3 patrolTarget;
    private bool isChasing = false;
    private float zombieSoundTimer = 0f;
    private float zombieSoundInterval = 5f;

    private bool isDead = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerHealth = player.GetComponent<HealthController>();

        SetRandomPatrolTarget();
    }

    void Update()
    {
        if (isDead || player == null || playerHealth == null || playerHealth.IsDead()) return;

        attackTimer += Time.deltaTime;
        zombieSoundTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            timeSinceLastSawPlayer = 0f;
            AttackPlayer();
        }
        else if (distance <= detectionRadius)
        {
            isChasing = true;
            timeSinceLastSawPlayer = 0f;
            ChasePlayer();
        }
        else if (isChasing)
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            if (timeSinceLastSawPlayer <= chaseMemoryTime && distance <= giveUpChaseDistance)
                ChasePlayer();
            else
                Patrol();
        }
        else
        {
            Patrol();
        }

        if (zombieSound != null && zombieSoundTimer >= zombieSoundInterval)
        {
            audioSource.PlayOneShot(zombieSound);
            zombieSoundTimer = 0f;
        }
    }

    public void KillZombie() 
    {
        isDead = true;
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    void ChasePlayer()
    {
        if (!agent.enabled) return;

        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        animator.SetBool("walk", false);
        animator.SetBool("run", true);
        animator.SetBool("attack", false);
    }

    void AttackPlayer()
    {
        if (!agent.enabled) return;

        agent.SetDestination(transform.position);

        animator.SetBool("walk", false);
        animator.SetBool("run", false);
        animator.SetBool("attack", true);

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage); // default body

            if (attackSound != null)
                audioSource.PlayOneShot(attackSound);
        }
    }

    void Patrol()
    {
        if (!agent.enabled) return;

        agent.speed = patrolSpeed;
        animator.SetBool("walk", true);
        animator.SetBool("run", false);
        animator.SetBool("attack", false);

        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
            SetRandomPatrolTarget();

        agent.SetDestination(patrolTarget);
    }

    void SetRandomPatrolTarget()
    {
        Vector3 randomDir = Random.insideUnitSphere * 10f + transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            patrolTarget = hit.position;
    }
}
