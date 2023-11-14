using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
    public Transform[] spawnLocations;
    public EnemyObject enemyType;
    public float spawnRate;
    public float lastSpawnTime;
    public int initialSpawnCount;
    public int maxSpawnCount;
}
