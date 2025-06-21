using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth;
    protected bool isDead = false;

    public Animator animator;
    public float destroyDelay = 5f;
    public Transform healthBarPivot;

    protected NavMeshAgent agent;

    [Header("Hit Sounds")]
    public AudioClip headshotSound;
    public AudioClip bodyshotSound;
    private AudioSource audioSource;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        UpdateHealthBar();
    }

    public virtual void TakeDamage(int damage, string hitPart)
    {
        if (isDead) return;

        // 🔊 Play sound sesuai bagian yang kena
        PlayHitSound(hitPart);

        if (hitPart == "Head")
        {
            currentHealth = 0;
            Die(true);
        }
        else
        {
            currentHealth -= damage;
            if (currentHealth <= 0) Die(false);
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void PlayHitSound(string hitPart)
    {
        if (audioSource == null) return;

        if (hitPart == "Head" && headshotSound != null)
        {
            audioSource.PlayOneShot(headshotSound);
        }
        else if (hitPart == "Body" && bodyshotSound != null)
        {
            audioSource.PlayOneShot(bodyshotSound);
        }
    }

    protected void UpdateHealthBar()
    {
        if (healthBarPivot != null)
        {
            float percent = (float)currentHealth / maxHealth;
            healthBarPivot.localScale = new Vector3(percent, 1, 1);
        }
    }

    protected virtual void Die(bool headshot)
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        if (animator != null)
        {
            animator.SetBool("death_head", headshot);
            animator.SetBool("death_body", !headshot);
        }

        ZombieMission mission = FindFirstObjectByType<ZombieMission>();
        if (mission != null)
        {
            mission.OnZombieKilled();
        }

        Destroy(gameObject, destroyDelay);
    }

    public bool IsDead() => isDead;
}
