using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Gun equipedGun;
    private Vector3 startPosition;

    public void EquipGun(Gun gun) => equipedGun = gun;

    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > equipedGun.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        // Apply damage and elemental effect to enemies
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(equipedGun.ammoType.damage);
            enemy.ApplyElementalEffect(equipedGun.elementalEffect);
        }

        Destroy(gameObject);  // Destroy bullet on collision
    }
}
