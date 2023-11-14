using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyObject EnemyData;
    private NavMeshAgent agent;
    private Transform target;
    private float lastFireTime;

    public void InitializeEnemy(EnemyObject data)
    {
        EnemyData = data;
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform; // Make sure to tag your player object.

        agent.speed = data.moveSpeed;
        lastFireTime = Time.time - 1 / data.fireRate; // Initialize the last fire time so the enemy can fire immediately.

        // Initialize other properties from EnemyObject if needed...
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (distanceToTarget <= EnemyData.agroRange)
        {
            agent.SetDestination(target.position);

            if (Time.time >= lastFireTime + 1 / EnemyData.fireRate)
            {
                // Implement firing logic here using enemyData.ammoType and enemyData.weapontype
                lastFireTime = Time.time;
            }
        }
        else
        {
            // Optionally implement behavior for when the player is out of aggro range
        }
    }

    public void Deactivate()
    {
        // Perform any cleanup or reset operations here
        EnemyPoolManager.Instance.ReturnEnemyToPool(EnemyData.enemyName, gameObject);
    }
}
