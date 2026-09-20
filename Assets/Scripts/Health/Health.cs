using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead;

    public static event System.Action OnPlayerDied;
    public event System.Action<int> OnHealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    [Tooltip("Flash the sprite red briefly when damaged.")]
    [SerializeField] private bool flashOnHit = true;
    [SerializeField] private float flashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    private static readonly Color HitColor = Color.red;

    public UnityEvent OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void AddBonusHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        if (flashOnHit)
        {
            FlashHit();
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    private void FlashHit()
    {
        if (spriteRenderer == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = HitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = Color.white;
        flashCoroutine = null;
    }

    private void Die()
    {
        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has died!");
            OnPlayerDied?.Invoke();
            Destroy(gameObject);
        }
        else
        {
            // Award score to player if this enemy has a score value
            EnemyScoreValue enemy = GetComponent<EnemyScoreValue>();
            if (enemy != null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    PlayerScore playerScore = playerObj.GetComponent<PlayerScore>();
                    if (playerScore != null)
                    {
                        playerScore.AddScore(enemy.ScoreValue);
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
