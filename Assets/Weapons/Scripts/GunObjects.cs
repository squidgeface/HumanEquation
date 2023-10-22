using UnityEngine;

public enum ElementType
{
    none,
    Fire,
    Water,
    Air,
    Earth
}

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun")]
public class Gun : ScriptableObject
{
    public string gunName;
    public Ammo ammoType;
    public ElementType elementalEffect;
    public float bulletSpeed;
    public float fireRate;
    public float range;
    // Other weapon properties...
}

