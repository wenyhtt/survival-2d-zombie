using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Update()
    {
        EnemySpawner spawner = EnemySpawner.Instance;
        if (spawner == null) return;

        if (spawner.IsGameComplete)
        {
            if (waveText != null) waveText.text = "All Waves Cleared!";
            if (enemiesText != null) enemiesText.gameObject.SetActive(false);
            if (countdownText != null) countdownText.gameObject.SetActive(false);
            return;
        }

        if (waveText != null)
            waveText.text = $"Wave: {spawner.CurrentWaveNumber}";

        bool countingDown = spawner.CurrentState == EnemySpawner.SpawnState.CountingDown;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(countingDown);
            countdownText.text = $"Next wave in: {Mathf.CeilToInt(spawner.WaveCountdown)}s";
        }

        if (enemiesText != null)
        {
            enemiesText.gameObject.SetActive(!countingDown);
            enemiesText.text = $"Enemies: {spawner.EnemiesRemaining}";
        }
    }
}
