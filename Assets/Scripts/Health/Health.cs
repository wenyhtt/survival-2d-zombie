using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Tooltip("Flash the sprite red briefly when damaged.")]
    [SerializeField] private bool flashOnHit = true;
    [SerializeField] private float flashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    private static readonly Color HitColor = Color.red;

    public UnityEvent OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AddBonusHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        if (flashOnHit)
        {
            FlashHit();
        }

        if (currentHealth <= 0)
        {
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
            Destroy(gameObject);
        }
        else
        {
            // Award score to player if this enemy has a score value
            EnemyAI enemy = GetComponent<EnemyAI>();
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
