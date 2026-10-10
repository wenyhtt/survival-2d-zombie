using UnityEngine;
using TMPro;

/// <summary>
/// Menampilkan nyawa (health) pemain saat ini di elemen UI teks.
/// Merubahan health dari komponen Health milik pemain.
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Tooltip("Text component to display health as a number")]
    [SerializeField] private TextMeshProUGUI healthText;

    private Health playerHealth;

    /// <summary>
    /// Diinisialisasi saat mulai, mencari objek pemain, mendapatkan komponen Health.
    /// </summary>
    private void Start()
    {
        // Mencari objek pemain dan mengambil komponen Health-nya
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Mendaftar ke peristiwa perubahan health
                playerHealth.OnHealthChanged += UpdateHealthUI;

                // Menginisialisasi UI dengan nilai awal
                UpdateHealthUI(playerHealth.CurrentHealth);
            }
        }
    }

    /// <summary>
    /// Dipanggil saat objek dihancurkan, membatalkan pendaftaran dari peristiwa perubahan health.
    /// </summary>
    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            // Selalu membatalkan pendaftaran untuk mencegah kebocoran memori
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    /// <summary>
    /// Memperbarui teks UI health dengan nilai health saat ini.
    /// </summary>
    private void UpdateHealthUI(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
    }
}
