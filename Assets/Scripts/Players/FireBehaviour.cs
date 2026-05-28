using System.Collections.Generic;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    private float downwardSpeedThreshold = 5.5f;
    private float gestureCheckInterval = 0.05f;
    private float cooldown = 0.5f;

    private float lastCheckedY;
    private float gestureTimer = 0f;
    private float cooldownTimer = 0f;

    private List<GameObject> enemiesInRange = new List<GameObject>();
    private List<GameObject> grenadesInRange = new List<GameObject>(); 
    private List<GameObject> shieldsInRange = new List<GameObject>(); 
    
    public int playerScore = 0;

    void Start()
    {
        lastCheckedY = transform.localPosition.y;
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
            // Track localPosition instead of world position
            float currentY = transform.localPosition.y;
            float yDelta = currentY - lastCheckedY;
            float verticalVelocity = yDelta / gestureTimer;

            // If it still misfires, try changing downwardSpeedThreshold from 2.5f to 4.0f
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
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayShootSound();
        }
        grenadesInRange.RemoveAll(grenade => grenade == null);
        shieldsInRange.RemoveAll(shield => shield == null);
        enemiesInRange.RemoveAll(enemy => enemy == null);

        if (grenadesInRange.Count > 0 || shieldsInRange.Count > 0)
        {
            GameObject targetGrenade = GetClosestObject(grenadesInRange);
            GameObject targetShield = GetClosestObject(shieldsInRange);

            GameObject targetItem = null;
            bool isGrenade = false;

            if (targetGrenade != null && targetShield != null)
            {
                float distG = Vector3.Distance(transform.position, targetGrenade.transform.position);
                float distS = Vector3.Distance(transform.position, targetShield.transform.position);
                if (distG < distS) { targetItem = targetGrenade; isGrenade = true; }
                else { targetItem = targetShield; isGrenade = false; }
            }
            else if (targetGrenade != null) { targetItem = targetGrenade; isGrenade = true; }
            else if (targetShield != null) { targetItem = targetShield; isGrenade = false; }

            if (targetItem != null)
            {
                cooldownTimer = cooldown;

                if (isGrenade)
                {
                    Explosion grenadeScript = targetItem.GetComponentInChildren<Explosion>();
                    if (grenadeScript != null)
                    {
                        playerScore += grenadeScript.detonate();
                    }
                    grenadesInRange.Remove(targetItem);
                }
                else
                {
                    Shield shieldScript = targetItem.GetComponent<Shield>();
                    if (shieldScript != null)
                    {
                        playerScore += 5;
                        shieldScript.ActivateShield(); 
                    }
                    shieldsInRange.Remove(targetItem);
                }

                Destroy(targetItem);
                return; 
            }
        }

        if (enemiesInRange.Count > 0)
        {
            GameObject targetEnemy = GetClosestObject(enemiesInRange);

            if (targetEnemy != null)
            {
                cooldownTimer = cooldown;
                
                ZombieAI enemyScript = targetEnemy.GetComponent<ZombieAI>();
                
                if (enemyScript != null && !enemyScript.IsDead)
                {
                    enemyScript.KillEnemy();
                    playerScore += enemyScript.pointsPerKill; 
                }
                
                enemiesInRange.Remove(targetEnemy);
            }
        }
    }

    private GameObject GetClosestObject(List<GameObject> listToSearch)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (GameObject obj in listToSearch)
        {
            if (obj != null)
            {
                ZombieAI enemyScript = obj.GetComponent<ZombieAI>();
                if (enemyScript != null && enemyScript.IsDead) continue;

                float distance = Vector3.Distance(obj.transform.position, playerPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = obj;
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
        else if (other.CompareTag("Grenade"))
        {
            if (!grenadesInRange.Contains(other.gameObject))
            {
                grenadesInRange.Add(other.gameObject);
            }
        }
        else if (other.CompareTag("Shield")) 
        {
            if (!shieldsInRange.Contains(other.gameObject))
            {
                shieldsInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") || other.gameObject.tag == "Untagged")
        {
            enemiesInRange.Remove(other.gameObject);
        }
        else if (other.CompareTag("Grenade"))
        {
            grenadesInRange.Remove(other.gameObject);
        }
        else if (other.CompareTag("Shield"))
        {
            shieldsInRange.Remove(other.gameObject);
        }
    }
}