using TMPro;
using UnityEngine;

/// <summary>
/// Menampilkan informasi gelombang musuh (nomor wave, jumlah musuh tersisa, dan hitung mundur) di UI.
/// </summary>
public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI countdownText;

    /// <summary>
    /// Memperbarui tampilan antarmuka gelombang setiap frame, termasuk nomor gelombang, hitungan mundur, dan sisa musuh.
    /// </summary>
    private void Update()
    {
        EnemySpawner spawner = EnemySpawner.Instance;
        if (spawner == null) return;

        if (waveText != null)
            waveText.text = $"{spawner.CurrentWaveNumber}";

        bool countingDown = spawner.CurrentState == EnemySpawner.SpawnState.CountingDown;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(countingDown);
            countdownText.text = $"Babak baru - {Mathf.CeilToInt(spawner.WaveCountdown)}s";
        }

        if (enemiesText != null)
        {
            enemiesText.transform.parent.gameObject.SetActive(!countingDown);
            enemiesText.text = $"{spawner.EnemiesRemaining}";
        }
    }
}
