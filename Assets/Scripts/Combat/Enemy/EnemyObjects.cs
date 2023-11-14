using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyObject : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public Weapon weaponType;
    public ElementType elementalEffect;
    public int maxHealth;
    public float moveSpeed;
    public float agroRange;
    public int initialPoolSize;
    // Other enemy properties...
}

