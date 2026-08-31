using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public UnityEvent OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
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

        if (currentHealth <= 0)
        {
            Die();
        }
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
