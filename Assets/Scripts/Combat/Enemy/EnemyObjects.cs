using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyObject : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public Ammo ammoType;
    public Weapon weapontype;
    public ElementType elementalEffect;
    public int maxHealth;
    public float moveSpeed;
    public float fireRate;
    public float agroRange;
    public int initialPoolSize;
    // Other enemy properties...
}

