using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnState { CountingDown, Spawning, WaitingForDeath }

    public static EnemySpawner Instance { get; private set; }

    [Header("Wave Settings")]
    [SerializeField] private int baseEnemyCount = 25;
    [SerializeField] private int waveIncrementMin = 3;
    [SerializeField] private int waveIncrementMax = 8;
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int maxEnemiesAlive = 25;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 areaSize = new Vector2(5f, 5f);
    [SerializeField] private bool spawnOnEdge = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);

    private int currentWaveIndex;
    private int currentWaveEnemyCount;
    private int enemiesSpawnedThisWave;
    private float waveCountdown;
    private SpawnState state = SpawnState.CountingDown;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private List<GameObject> shuffledPrefabs = new List<GameObject>();
    private GameObject currentWavePrefab;

    public SpawnState CurrentState => state;
    public int CurrentWaveNumber => currentWaveIndex + 1;
    public float WaveCountdown => waveCountdown;
    public int EnemiesRemaining => Mathf.Max(0, currentWaveEnemyCount - enemiesSpawnedThisWave + spawnedEnemies.Count);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        waveCountdown = timeBetweenWaves;
        currentWaveEnemyCount = baseEnemyCount;
    }

    private void Update()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (state == SpawnState.CountingDown)
        {
            waveCountdown -= Time.deltaTime;
            if (waveCountdown <= 0f)
                StartWave();
        }
        else if (state == SpawnState.Spawning)
        {
            while (enemiesSpawnedThisWave < currentWaveEnemyCount && spawnedEnemies.Count < maxEnemiesAlive)
            {
                SpawnEnemy();
            }
        }
        else if (state == SpawnState.WaitingForDeath && spawnedEnemies.Count == 0)
        {
            CompleteWave();
        }
    }

    private void StartWave()
    {
        enemiesSpawnedThisWave = 0;
        currentWavePrefab = GetNextWavePrefab();

        if (currentWaveEnemyCount <= 0 || currentWavePrefab == null || maxEnemiesAlive <= 0)
            CompleteWave();
        else
            state = SpawnState.Spawning;
    }

    private void CompleteWave()
    {
        currentWaveIndex++;
        int increment = Random.Range(waveIncrementMin, waveIncrementMax + 1);
        currentWaveEnemyCount += increment;

        Debug.Log($"Wave {currentWaveIndex} complete. Next wave: {currentWaveEnemyCount} enemies (+{increment})");

        waveCountdown = timeBetweenWaves;
        state = SpawnState.CountingDown;
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(currentWavePrefab, GetSpawnPosition(), Quaternion.identity);
        spawnedEnemies.Add(newEnemy);
        enemiesSpawnedThisWave++;

        Health enemyHealth = newEnemy.GetComponent<Health>();
        if (enemyHealth != null && currentWaveIndex > 0)
        {
            int bonusPerWave = Random.Range(10, 21);
            int totalBonus = bonusPerWave * currentWaveIndex;
            enemyHealth.AddBonusHealth(totalBonus);
        }

        if (enemiesSpawnedThisWave >= currentWaveEnemyCount)
            state = SpawnState.WaitingForDeath;
    }

    private GameObject GetNextWavePrefab()
    {
        if (shuffledPrefabs.Count == 0)
            RefillShuffledPrefabs();

        if (shuffledPrefabs.Count == 0) return null;

        int lastIndex = shuffledPrefabs.Count - 1;
        GameObject prefab = shuffledPrefabs[lastIndex];
        shuffledPrefabs.RemoveAt(lastIndex);
        return prefab;
    }

    private void RefillShuffledPrefabs()
    {
        if (enemyPrefabs == null) return;

        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab != null)
                shuffledPrefabs.Add(prefab);
        }

        for (int i = shuffledPrefabs.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = shuffledPrefabs[i];
            shuffledPrefabs[i] = shuffledPrefabs[randomIndex];
            shuffledPrefabs[randomIndex] = temp;
        }
    }

    private Vector3 GetSpawnPosition()
    {
        if (!spawnOnEdge)
        {
            float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float randomY = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
            return transform.position + new Vector3(randomX, randomY, 0f);
        }

        float halfW = areaSize.x / 2f;
        float halfH = areaSize.y / 2f;
        int edge = Random.Range(0, 4);

        if (edge == 0) return transform.position + new Vector3(Random.Range(-halfW, halfW), halfH, 0f);
        if (edge == 1) return transform.position + new Vector3(Random.Range(-halfW, halfW), -halfH, 0f);
        if (edge == 2) return transform.position + new Vector3(-halfW, Random.Range(-halfH, halfH), 0f);
        return transform.position + new Vector3(halfW, Random.Range(-halfH, halfH), 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}
