using UnityEngine;


public enum ElementType
{
    none,
    Fire,
    Water,
    Air,
    Earth
}

public class Weapon : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public ElementType elementalEffect;
    public int damage;
    public float attackRate;
    public float attackRange;
}
