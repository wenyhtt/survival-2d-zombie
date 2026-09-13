using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Tooltip("Text component to display health as a number")]
    [SerializeField] private TextMeshProUGUI healthText;

    private Health playerHealth;

    private void Start()
    {
        // Find the player object and grab its Health component
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Subscribe to the health changed event
                playerHealth.OnHealthChanged += UpdateHealthUI;
                
                // Initialize the UI with the starting value
                UpdateHealthUI(playerHealth.CurrentHealth);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            // Always unsubscribe to prevent memory leaks
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }
}
