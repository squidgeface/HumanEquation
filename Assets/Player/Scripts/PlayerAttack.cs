using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Gun gun;
    public Weapon weapon;
    public LayerMask enemyLayers;
    public Transform firePoint;
    public Transform attackPoint;
    public Transform attackTarget;

    private float lastShotTime = 0;

    private bool isShooting;
    private bool isMeleeAttacking;

    void Update()
    {
        //if (isShooting && Time.time >= lastShotTime + gun.fireRate)
        //{
        //    lastShotTime = Time.time;
        //    Shoot();
        //}

        //if (isMeleeAttacking)
        //{
        //    MeleeAttack();
        //    isMeleeAttacking = false;  // Reset flag
        //}
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isShooting = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isShooting = false;
        }
    }

    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            isMeleeAttacking = true;
    }

    void Shoot()
    {
        if (gun.ammoType == null) return;
     
        GameObject bullet = Instantiate(gun.ammoType.bulletPrefab, firePoint.position, firePoint.rotation, null);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        Vector3 fireDirection = (attackTarget.position - firePoint.position).normalized;
        bulletRb.velocity = fireDirection * gun.bulletSpeed;

        // Assuming Bullet has a method to set its damage and elemental effect
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDamage(gun.ammoType.damage);
        bulletScript.SetElementalEffect(gun.elementalEffect);
        bulletScript.setDistance(firePoint.position, gun.range);
    }

    void MeleeAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, weapon.range, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            // Assume enemy has a method to take damage
            enemy.GetComponent<Enemy>().TakeDamage(10);  // Example damage value
        }
    }

    private bool StartTimer(ref float timer, float targetTime)
    {
        timer += Time.deltaTime;
        if (timer > targetTime)
        {
            timer = 0;
            return true;
        }
        return false;
    }
}
