using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    private float downwardSpeedThreshold = 2.5f;
    private float gestureCheckInterval = 0.05f;
    private float cooldown = 0.5f;

    private float lastCheckedY;
    private float gestureTimer = 0f;
    private float cooldownTimer = 0f;

    private List<GameObject> enemiesInRange = new List<GameObject>();
    public int playerScore = 0;
    public int pointsPerKill = 10;

    void Start()
    {
        lastCheckedY = transform.position.y;
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        HandleGestureDetection();
    }

    private void HandleGestureDetection()
    {
        gestureTimer += Time.deltaTime;

        if (gestureTimer >= gestureCheckInterval)
        {
            float currentY = transform.position.y;
            float yDelta = currentY - lastCheckedY;
            float verticalVelocity = yDelta / gestureTimer;

            if (verticalVelocity < -downwardSpeedThreshold && cooldownTimer <= 0)
            {
                TriggerShootingMechanic();
            }

            lastCheckedY = currentY;
            gestureTimer = 0f;
        }
    }

    private void TriggerShootingMechanic()
    {
        enemiesInRange.RemoveAll(enemy => enemy == null);

        if (enemiesInRange.Count > 0)
        {
            GameObject targetEnemy = GetClosestEnemy();

            if (targetEnemy != null)
            {
                cooldownTimer = cooldown;
                
                ZombieAI enemyScript = targetEnemy.GetComponent<ZombieAI>();
                
                if (enemyScript != null && !enemyScript.IsDead)
                {
                    enemyScript.KillEnemy();
                    playerScore += pointsPerKill;
                }
                
                enemiesInRange.Remove(targetEnemy);
            }
        }
    }

    private GameObject GetClosestEnemy()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                ZombieAI enemyScript = enemy.GetComponent<ZombieAI>();
                if (enemyScript != null && enemyScript.IsDead) continue;

                float distance = Vector3.Distance(enemy.transform.position, playerPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = enemy;
                }
            }
        }

        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            ZombieAI enemyScript = other.GetComponent<ZombieAI>();
            
            if (enemyScript != null && enemyScript.IsDead) return;

            if (!enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") || other.gameObject.tag == "Untagged")
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}