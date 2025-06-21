using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;

    public Transform healthBarPivot;
    private Animator animator;
    private bool isDead = false;

    [Header("UI")]
    public GameObject pauseMenuPanel;
    public GameObject hitEffectImage;

    [Header("Audio")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("[PlayerHealth] Animator tidak ditemukan!");
        }

        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (hitEffectImage != null) hitEffectImage.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"[PlayerHealth] Terkena damage: {damage}, darah sekarang: {currentHealth}");

        // 🔊 Play hit sound only when being attacked
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarPivot != null)
        {
            float percent = (float)currentHealth / maxHealth;
            healthBarPivot.localScale = new Vector3(percent, 1f, 1f);
        }
        else
        {
            Debug.LogWarning("[PlayerHealth] HealthBarPivot belum diset!");
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player mati.");

        if (animator != null)
        {
            animator.SetBool("Death", true);
        }

        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        if (hitEffectImage != null)
        {
            hitEffectImage.SetActive(false);
        }

        // 🔇 Stop semua AudioSource yang aktif di scene
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Stop();
        }
    }


    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"[PlayerHealth] Disembuhkan sebanyak: {amount}, darah sekarang: {currentHealth}");

        UpdateHealthBar();
    }

    public bool IsHealthFull()
    {
        return currentHealth >= maxHealth;
    }

    public void EnableHitEffect()
    {
        if (hitEffectImage != null)
            hitEffectImage.SetActive(true);
    }

    public void DisableHitEffect()
    {
        if (hitEffectImage != null)
            hitEffectImage.SetActive(false);
    }
}
