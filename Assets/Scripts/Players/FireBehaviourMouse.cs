using System.Collections.Generic;
using UnityEngine;

public class FireBehaviourMouse : MonoBehaviour
{
    [Header("Mouse Movement")]
    public Camera mainCamera;
    public float fixedY = 1f;
    public bool useCurrentYAsFixedY = true;

    [Header("Shooting")]
    public float cooldown = 0.2f;

    private float cooldownTimer = 0f;

    private List<GameObject> enemiesInRange = new List<GameObject>();
    private List<GameObject> grenadesInRange = new List<GameObject>();
    private List<GameObject> shieldsInRange = new List<GameObject>();

    public int playerScore = 0;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (useCurrentYAsFixedY)
        {
            fixedY = transform.position.y;
        }
    }

    void Update()
    {
        MoveHorizontallyWithMouse();

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        TryAutoShoot();
    }

    private void MoveHorizontallyWithMouse()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        Plane horizontalPlane = new Plane(Vector3.up, new Vector3(0f, fixedY, 0f));

        if (horizontalPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);

            transform.position = new Vector3(
                mouseWorldPosition.x,
                transform.position.y,
                transform.position.z
            );
        }
    }

    private void TryAutoShoot()
    {
        if (cooldownTimer > 0) return;

        enemiesInRange.RemoveAll(enemy => enemy == null);
        grenadesInRange.RemoveAll(grenade => grenade == null);
        shieldsInRange.RemoveAll(shield => shield == null);

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

                if (distG < distS)
                {
                    targetItem = targetGrenade;
                    isGrenade = true;
                }
                else
                {
                    targetItem = targetShield;
                    isGrenade = false;
                }
            }
            else if (targetGrenade != null)
            {
                targetItem = targetGrenade;
                isGrenade = true;
            }
            else if (targetShield != null)
            {
                targetItem = targetShield;
                isGrenade = false;
            }

            if (targetItem != null)
            {
                cooldownTimer = cooldown;

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayShootSound();
                }

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
                ZombieAI enemyScript = targetEnemy.GetComponent<ZombieAI>();

                if (enemyScript != null && !enemyScript.IsDead)
                {
                    cooldownTimer = cooldown;

                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlayShootSound();
                    }

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

                if (enemyScript != null && enemyScript.IsDead)
                {
                    continue;
                }

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

            if (enemyScript != null && enemyScript.IsDead)
            {
                return;
            }

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

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
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