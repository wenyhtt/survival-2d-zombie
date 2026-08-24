using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemiesAlive = 10;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 areaSize = new Vector2(5f, 5f);
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);

    private float timer;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Update()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // Clean up the list to remove enemies that have been destroyed
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        // Check if we haven't reached the max limit
        if (spawnedEnemies.Count < maxEnemiesAlive)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
    }

    private void SpawnEnemy()
    {
        // Pick a random enemy prefab from the list
        GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Calculate a random position within the defined area
        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomY = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);
        Vector3 spawnPos = transform.position + new Vector3(randomX, randomY, 0f);

        // Spawn the enemy and add it to our list
        GameObject newEnemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        spawnedEnemies.Add(newEnemy);
    }

    // This draws a box in the Unity Editor so you can visually see the spawn area
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}
