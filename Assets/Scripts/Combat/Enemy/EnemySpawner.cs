using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public SpawnPoint[] spawnPoints;

    private void Start()
    {
        InitializeSpawning();
    }

    private void InitializeSpawning()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            for (int i = 0; i < spawnPoint.initialSpawnCount; i++)
            {
                SpawnEnemy(spawnPoint, immediate: true);
            }
        }
    }

    private void Update()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (Time.time >= spawnPoint.lastSpawnTime + spawnPoint.spawnRate)
            {
                SpawnEnemy(spawnPoint, immediate: false);
                spawnPoint.lastSpawnTime = Time.time;
            }
        }
    }

    private void SpawnEnemy(SpawnPoint spawnPoint, bool immediate)
    {
        foreach (var location in spawnPoint.spawnLocations)
        {
            var enemyName = spawnPoint.enemyType.enemyName;
            if (EnemyPoolManager.Instance.TotalSpawnedEnemies.ContainsKey(enemyName) && EnemyPoolManager.Instance.TotalSpawnedEnemies[enemyName] >= spawnPoint.maxSpawnCount) return;

            GameObject enemyGO = EnemyPoolManager.Instance.GetPooledEnemy(enemyName);
            if (enemyGO != null)
            {
                enemyGO.transform.position = location.position;
                enemyGO.transform.rotation = location.rotation;

                Enemy enemyScript = enemyGO.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.InitializeEnemy(spawnPoint.enemyType);
                }

                if (!immediate)
                {
                    // If not immediate, deactivate the enemy and allow the regular spawn process
                    enemyGO.SetActive(false);
                    EnemyPoolManager.Instance.ReturnEnemyToPool(enemyName, enemyGO);
                }
            }
            else
            {
                // Optionally handle the case where no enemy is available in the pool
            }
        }
    }
}
