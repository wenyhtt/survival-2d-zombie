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
