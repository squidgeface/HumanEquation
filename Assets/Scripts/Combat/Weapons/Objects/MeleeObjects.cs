using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Melee")]
public class Melee : Weapon
{
    public Transform damageCenter;
    public GameObject slashEffectPrefab;
    // Other weapon properties...
}
