using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class HealthController : MonoBehaviour
{
    [Header("Type")]
    public bool isPlayer = false;
    public bool isAiOrDummy = false;

    [Header("HP & Shield")]
    public float health = 100f;
    public float shield = 50f;

    private float initialHealth;
    private float initialShield;

    [Header("UI Pivots")]
    public Transform healthBarPivot;
    public Transform shieldBarPivot;

    [Header("Animation & Agent")]
    public Animator animator;
    public NavMeshAgent agent;
    public float destroyDelay = 5f;

    [Header("Ragdoll (Player Only)")]
    public GameObject ragdollPrefab;

    [Header("UI Death (Player Only)")]
    public GameObject youAreDeadUI;
    public float delayToMainMenu = 5f;

    [Header("UI Hit Effect (Player Only)")]
    public GameObject hitEffectImage; // Tambahkan UI image efek hit (durasi 1 detik)

    [Header("Back Button Countdown")]
    public Button backToMenuButton;
    public Text backToMenuText;
    public float countdownTime = 10f;

    private bool returnInitiated = false;

    private bool isDead = false;

    void Start()
    {
        initialHealth = health;
        initialShield = shield;

        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (hitEffectImage != null)
            hitEffectImage.SetActive(false);

        UpdateBars();
    }

    void UpdateBars()
    {
        if (healthBarPivot != null)
        {
            float percent = health / initialHealth;
            healthBarPivot.localScale = new Vector3(percent, 1f, 1f);
        }

        if (shieldBarPivot != null)
        {
            float percent = shield / initialShield;
            shieldBarPivot.localScale = new Vector3(percent, 1f, 1f);
        }
    }

    public void TakeDamage(float damage, string hitPart = "Body")
    {
        if (isDead) return;

        if (hitPart == "Head")
        {
            health = 0;
            Die(true);
            return;
        }

        if (shield > 0)
        {
            float absorbed = Mathf.Min(shield, damage);
            shield -= absorbed;
            damage -= absorbed;
        }

        if (damage > 0)
            health -= damage;

        health = Mathf.Max(health, 0);
        shield = Mathf.Max(shield, 0);

        UpdateBars();

        // 🔥 Tampilkan efek hit selama 1 detik
        if (isPlayer && hitEffectImage != null)
            StartCoroutine(ShowHitEffect());

        if (health <= 0)
            Die(false);
    }

    IEnumerator ShowHitEffect()
    {
        hitEffectImage.SetActive(true);
        yield return new WaitForSeconds(1f); // durasi sesuai animasi
        hitEffectImage.SetActive(false);
    }

    void Die(bool headshot)
    {
        if (isDead) return;
        isDead = true;

        // ✅ Matikan suara zombie kalau ada
        ZombieAI ai = GetComponent<ZombieAI>();
        if (ai != null)
            ai.KillZombie();

        if (isAiOrDummy && ZombieMissionManager.Instance != null)
            ZombieMissionManager.Instance.RegisterZombieKill();

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        if (isPlayer)
        {
            if (ragdollPrefab != null)
                Instantiate(ragdollPrefab, transform.position, transform.rotation);

            if (youAreDeadUI != null)
                youAreDeadUI.SetActive(true);

            if (!returnInitiated)
                StartCoroutine(StartBackToMenuCountdown()); // ✅ cukup ini saja
        }

        else
        {
            if (animator != null)
            {
                animator.SetBool("death_head", headshot);
                animator.SetBool("death_body", !headshot);
            }

            Destroy(gameObject, destroyDelay); // Tetap dihancurkan
        }
    }

    IEnumerator StartBackToMenuCountdown()
    {
        returnInitiated = true;

        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(() => SceneManager.LoadScene("Main Menu"));

        float currentTime = countdownTime;
        while (currentTime > 0)
        {
            if (backToMenuText != null)
                backToMenuText.text = $"Back to Main Menu ({Mathf.CeilToInt(currentTime)})";

            currentTime -= Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Main Menu");
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        health += amount;
        health = Mathf.Min(health, initialHealth); // Biar tidak lebih dari max HP

        UpdateBars(); // Update UI bar HP
    }



    public bool IsDead() => isDead;
}
