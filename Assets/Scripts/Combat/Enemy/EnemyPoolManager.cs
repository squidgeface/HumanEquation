using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    [SerializeField] private EnemyObject[] enemyTypes; // Assign different enemy types in the inspector
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, float> TotalSpawnedEnemies = new Dictionary<string, float>();

    [HideInInspector] public EnemyObject[] EnemyTypes => enemyTypes;
    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    // Create a pool for each type of enemy
    private void InitializePool()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var enemy in enemyTypes)
        {
            Queue<GameObject> enemyPool = new Queue<GameObject>();
            for (int i = 0; i < enemy.initialPoolSize; i++)
            {
                GameObject enemyGO = Instantiate(enemy.enemyPrefab);
                enemyGO.SetActive(false);
                enemyPool.Enqueue(enemyGO);
            }
            poolDictionary.Add(enemy.enemyName, enemyPool);
        }
    }

    // Ensure this method can handle creating new pooled enemies if needed
    public GameObject GetPooledEnemy(string enemyName)
    {
        if (!poolDictionary.ContainsKey(enemyName) || poolDictionary[enemyName].Count == 0)
        {
            // Find the EnemyObject by name
            EnemyObject enemyType = System.Array.Find(enemyTypes, type => type.enemyName == enemyName);
            if (enemyType != null)
            {
                GameObject newEnemy = Instantiate(enemyType.enemyPrefab);
                newEnemy.SetActive(false);
                poolDictionary[enemyName].Enqueue(newEnemy);
                return newEnemy;
            }
            else
            {
                Debug.LogWarning($"Enemy type {enemyName} not found.");
                return null;
            }
        }

        if (TotalSpawnedEnemies.ContainsKey(enemyName))
            TotalSpawnedEnemies[enemyName]++;
        else
            TotalSpawnedEnemies.Add(enemyName, 0);

        GameObject enemyToSpawn = poolDictionary[enemyName].Dequeue();
        enemyToSpawn.SetActive(true);
        return enemyToSpawn;
    }

    // Return an enemy to the pool
    public void ReturnEnemyToPool(string enemyName, GameObject enemy)
    {
        enemy.SetActive(false);
        poolDictionary[enemyName].Enqueue(enemy);
        TotalSpawnedEnemies[enemyName]--;
    }
}
