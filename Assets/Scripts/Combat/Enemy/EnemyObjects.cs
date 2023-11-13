using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyObject : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public Ammo ammoType;
    public Weapon weapontype;
    public ElementType elementalEffect;
    public float moveSpeed;
    public float fireRate;
    public float agroRange;
    // Other enemy properties...
}

