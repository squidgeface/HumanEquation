using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("WeaponInventory")]
    [SerializeField] private Transform gunSpawnTransform;
    [SerializeField] private List<Gun> gunList;
    [SerializeField] private Gun equipedGun;
    [SerializeField] private Melee equipedMelee;
    private int equipedGunReference;
    private GameObject equipedGunInstance;
    private float lastFireTime;
    private float lastMeleeTime;

    private void Start()
    {
        if (gunList.Count > 0)
        {
            EquipGun(gunList[0]);
            lastFireTime = -equipedGun.attackRate; // Ensure the gun is ready to fire immediately
            equipedGunReference = 0;
        }
    }

    public void TryFire()
    {
        if (equipedGun != null && Time.time >= lastFireTime + (1 / equipedGun.attackRate))
        {
            Fire();
        }
    }

    public void TryMelee()
    {
        if (equipedMelee != null && Time.time >= lastMeleeTime + (1 / equipedMelee.attackRate))
        {
            MeleeAttack();
        }
    }

    // Method to fire the currently equipped gun
    public void Fire()
    {
        if (equipedGun == null || equipedGun.bulletEmitter == null)
        {
            Debug.LogWarning("WeaponManager: Missing components for firing.");
            return;
        }

        // Instantiate the bullet from the ammo's prefab at the gun muzzle position and rotation
        GameObject bullet = Instantiate(
            equipedGun.bulletPrefab,
            equipedGun.bulletEmitter.position,
            equipedGun.bulletEmitter.rotation
        );

        Bullet bulletSettings = bullet.GetComponent<Bullet>();
        bulletSettings.EquipGun(equipedGun);

        // Set the bullet's velocity
        bullet.GetComponent<Rigidbody>().velocity = equipedGun.bulletEmitter.forward * equipedGun.bulletSpeed;

        lastFireTime = Time.time; // Update the last fire time
        // If there's an elemental effect, you can pass it to the bullet here
    }

    // Method to fire the currently equipped gun
    public void MeleeAttack()
    {
        if (equipedMelee == null || equipedMelee.slashEffectPrefab == null)
        {
            Debug.LogWarning("WeaponManager: Missing components for attacking.");
            return;
        }

        // Instantiate the bullet from the ammo's prefab at the gun muzzle position and rotation
        GameObject slashEffect = Instantiate(
            equipedMelee.slashEffectPrefab,
            equipedMelee.damageCenter.position,
            equipedMelee.damageCenter.rotation
        );

        lastFireTime = Time.time; // Update the last fire time
        // If there's an elemental effect, you can pass it to the bullet here
    }

    public void SwitchGun(int direction)
    {
        var addition = direction > 0 ? 1 : -1;
        equipedGunReference += addition;
        if (equipedGunReference < 0)
            equipedGunReference = gunList.Count - 1;
        else if (equipedGunReference >= gunList.Count)
            equipedGunReference = 0;
        EquipGun(gunList[equipedGunReference]);
    }

    // Method to switch the currently equipped gun
    public void EquipGun(Gun newGun)
    {
        equipedGun = newGun;
        // Handle equipping the new gun visually and functionally
        InstantiateNewGun(equipedGun);
    }

    private void InstantiateNewGun(Gun newGun)
    {
        if (equipedGunInstance)
            Destroy(equipedGunInstance);
        equipedGunInstance = Instantiate(
            newGun.weaponPrefab,
            gunSpawnTransform.position,
            gunSpawnTransform.rotation,
            transform
        );
        // Gun prefab should have BulletEmitter as child
        newGun.bulletEmitter = equipedGunInstance.transform.GetChild(0);
    }

    // Add additional methods for handling weapon functionality as needed, such as reloading.
}
