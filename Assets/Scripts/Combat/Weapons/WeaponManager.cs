using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("WeaponInventory")]
    [SerializeField] private Transform gunSpawnTransform;
    [SerializeField] private Gun[] gunList;
    [SerializeField] private Gun equippedGun;
    private GameObject equippedGunInstance;
    private float lastFireTime;

    private void Start()
    {
        if (gunList.Length > 0)
        {
            EquipGun(gunList[0]);
            lastFireTime = -equippedGun.fireRate; // Ensure the gun is ready to fire immediately
        }
    }

    public void TryFire()
    {
        if (equippedGun != null && Time.time >= lastFireTime + (1 / equippedGun.fireRate))
        {
            Fire();
        }
    }

    // Method to fire the currently equipped gun
    public void Fire()
    {
        if (equippedGun == null || equippedGun.ammoType == null || equippedGun.bulletEmitter == null)
        {
            Debug.LogWarning("WeaponManager: Missing components for firing.");
            return;
        }

        // Instantiate the bullet from the ammo's prefab at the gun muzzle position and rotation
        GameObject bullet = Instantiate(
            equippedGun.ammoType.bulletPrefab,
            equippedGun.bulletEmitter.position,
            equippedGun.bulletEmitter.rotation
        );

        Bullet bulletSettings = bullet.GetComponent<Bullet>();
        bulletSettings.EquipGun(equippedGun);

        // Set the bullet's velocity
        bullet.GetComponent<Rigidbody>().velocity = equippedGun.bulletEmitter.forward * equippedGun.bulletSpeed;

        lastFireTime = Time.time; // Update the last fire time
        // If there's an elemental effect, you can pass it to the bullet here
    }

    // Method to switch the currently equipped gun
    public void EquipGun(Gun newGun)
    {
        equippedGun = newGun;
        // Handle equipping the new gun visually and functionally
        InstantiateNewGun(equippedGun);
    }

    private void InstantiateNewGun(Gun newGun)
    {
        if (equippedGunInstance)
            Destroy(equippedGunInstance);
        equippedGunInstance = Instantiate(
            newGun.gunPrefab,
            gunSpawnTransform.position,
            gunSpawnTransform.rotation,
            transform
        );
        // Gun prefab should have BulletEmitter as child
        newGun.bulletEmitter = equippedGunInstance.transform.GetChild(0);
    }

    // Add additional methods for handling weapon functionality as needed, such as reloading.
}
