using UnityEngine;


[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun")]
public class Gun : Weapon
{
    public Transform bulletEmitter;
    public float bulletSpeed;
    public GameObject bulletPrefab;
    // Other weapon properties...
}

