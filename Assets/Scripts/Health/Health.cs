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
            // For now, we'll just destroy the player object. You can hook up a Game Over screen later.
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
