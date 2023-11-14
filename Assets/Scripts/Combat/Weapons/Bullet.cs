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
        if (Vector3.Distance(startPosition, transform.position) > equipedGun.attackRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        HealthManager healthManager = collision.gameObject.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.TakeDamage(equipedGun.damage);
        }
        Destroy(gameObject);  // Destroy bullet on collision
    }
}
