using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mengelola sistem spawning (kemunculan) musuh berdasarkan gelombang (wave).
/// Mengontrol jumlah musuh per gelombang, jeda antar gelombang, dan logika penantian hingga semua musuh mati.
/// </summary>
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
    private Transform playerTransform;
    private Rigidbody2D playerRigidbody;
    private Vector2 playerSpawnPosition;
    private bool hasPlayerSpawnPosition;

    public SpawnState CurrentState => state;
    public int CurrentWaveNumber => currentWaveIndex + 1;
    public float WaveCountdown => waveCountdown;
    public int EnemiesRemaining => Mathf.Max(0, currentWaveEnemyCount - enemiesSpawnedThisWave + spawnedEnemies.Count);

    /// <summary>
    /// Menginisialisasi instance singleton dari EnemySpawner.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Menghapus referensi instance singleton saat objek dihancurkan.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Menginisialisasi pengaturan gelombang awal dan menyimpan posisi spawn pemain.
    /// </summary>
    private void Start()
    {
        waveCountdown = timeBetweenWaves;
        currentWaveEnemyCount = baseEnemyCount;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerRigidbody = player.GetComponent<Rigidbody2D>();
            playerSpawnPosition = playerRigidbody != null
                ? playerRigidbody.position
                : playerTransform.position;
            hasPlayerSpawnPosition = true;
        }
    }

    /// <summary>
    /// Memperbarui state spawner setiap frame, menangani countdown gelombang, proses spawn, dan penyelesaian gelombang.
    /// </summary>
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

    /// <summary>
    /// Memulai gelombang baru dengan mereset jumlah musuh yang dispawn dan mengambil prefab musuh untuk gelombang ini.
    /// </summary>
    private void StartWave()
    {
        enemiesSpawnedThisWave = 0;
        currentWavePrefab = GetNextWavePrefab();

        if (currentWaveEnemyCount <= 0 || currentWavePrefab == null || maxEnemiesAlive <= 0)
            CompleteWave();
        else
            state = SpawnState.Spawning;
    }

    /// <summary>
    /// Menyelesaikan gelombang saat ini, mengembalikan pemain ke posisi awal, dan menyiapkan gelombang berikutnya.
    /// </summary>
    private void CompleteWave()
    {
        ReturnPlayerToSpawn();

        currentWaveIndex++;
        int increment = Random.Range(waveIncrementMin, waveIncrementMax + 1);
        currentWaveEnemyCount += increment;

        Debug.Log($"Wave {currentWaveIndex} complete. Next wave: {currentWaveEnemyCount} enemies (+{increment})");

        waveCountdown = timeBetweenWaves;
        state = SpawnState.CountingDown;
    }

    /// <summary>
    /// Mengembalikan pemain ke posisi spawn awalnya.
    /// </summary>
    private void ReturnPlayerToSpawn()
    {
        if (!hasPlayerSpawnPosition)
            return;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            playerTransform = player.transform;
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.position = playerSpawnPosition;
            playerRigidbody.linearVelocity = Vector2.zero;
        }
        else
        {
            Vector3 position = playerTransform.position;
            playerTransform.position = new Vector3(playerSpawnPosition.x, playerSpawnPosition.y, position.z);
        }
    }

    /// <summary>
    /// Memunculkan satu musuh di posisi acak dan menambahkannya ke daftar musuh yang telah muncul.
    /// </summary>
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

    /// <summary>
    /// Mengambil prefab musuh selanjutnya untuk dimunculkan dalam gelombang ini, mengocok ulang jika perlu.
    /// </summary>
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

    /// <summary>
    /// Mengisi kembali dan mengocok daftar prefab musuh yang tersedia.
    /// </summary>
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

    /// <summary>
    /// Mengkalkulasi posisi acak di dalam area atau di tepi area untuk memunculkan musuh.
    /// </summary>
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

    /// <summary>
    /// Menggambar area batas kemunculan musuh di dalam editor Unity.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}
