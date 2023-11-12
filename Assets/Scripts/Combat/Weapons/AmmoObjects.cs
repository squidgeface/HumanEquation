using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "Weapons/Ammo")]
public class Ammo : ScriptableObject
{
    public string ammoName;
    public int damage;
    public GameObject bulletPrefab;
    // Other ammo properties...
}