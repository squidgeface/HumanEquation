using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _bulletDamage;
    private ElementType _elementalEffect;
    private Vector3 _startPosition;
    private float _fireDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _fireDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(int damage)
    {
        _bulletDamage = damage;
    }

    public void SetElementalEffect(ElementType elementType)
    {
        _elementalEffect = elementType;
    }

    public void setDistance(Vector3 startPosition, float distance) 
    {
        _startPosition = startPosition;
        _fireDistance = distance;
    }

    void OnTriggerEnter(Collider other)
    {
        // Apply damage and elemental effect to enemies
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(_bulletDamage);
            enemy.ApplyElementalEffect(_elementalEffect);
        }

        Destroy(gameObject);  // Destroy bullet on collision
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
