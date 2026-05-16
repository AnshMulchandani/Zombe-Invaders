using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private string deathTriggerName = "Die";
    public float despawnDelay = 2.5f;
    private string towerTag = "Tower";
    public float attackRange = 2.5f;
    public float damagePerAttack = 15f;
    public float attackCooldown = 1.2f;
    private string attackBoolName = "IsAttacking";

    private Animator animator;
    private Collider enemyCollider;
    private NavMeshAgent agent;

    private Tower targetTowerHealth;
    private float attackTimer = 0f;
    private bool isDead = false;

    public bool IsDead => isDead;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (agent != null)
        {
            agent.stoppingDistance = attackRange - 0.2f;
        }
        FindClosestTower();
    }

    public void SetTarget(Transform target)
    {
        if (isDead) return;

        if (agent != null && target != null)
        {
            agent.SetDestination(target.position);

            Tower towerHealth = target.GetComponent<Tower>();
            if (towerHealth != null)
            {
                targetTowerHealth = towerHealth;
            }
        }
    }

    void Update()
    {
        if (isDead) return;

        if (targetTowerHealth == null || !targetTowerHealth.gameObject.activeInHierarchy)
        {
            if (animator != null) animator.SetBool(attackBoolName, false);
            FindClosestTower();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetTowerHealth.transform.position);
        Collider towerCollider = targetTowerHealth.GetComponent<Collider>();
        
        if (towerCollider != null)
        {
            distanceToTarget = Vector3.Distance(transform.position, towerCollider.ClosestPoint(transform.position));
        }

        if (distanceToTarget <= attackRange)
        {
            if (agent != null && agent.isOnNavMesh) 
            {
                agent.isStopped = true;
            }

            if (animator != null) animator.SetBool(attackBoolName, true);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                ExecuteAttack();
                attackTimer = 0f;
            }
        }
        else
        {
            if (agent != null && agent.isOnNavMesh) 
            {
                agent.isStopped = false;
                agent.SetDestination(targetTowerHealth.transform.position);
            }

            if (animator != null) animator.SetBool(attackBoolName, false);
            attackTimer = 0f;
        }
    }

    private void FindClosestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag(towerTag);
        GameObject closestTower = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject tower in towers)
        {
            if (tower.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, tower.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTower = tower;
                }
            }
        }

        if (closestTower != null)
        {
            SetTarget(closestTower.transform);
        }
        else
        {
            targetTowerHealth = null;
            if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
            if (animator != null) animator.SetBool(attackBoolName, false);
        }
    }

    private void ExecuteAttack()
    {
        if (targetTowerHealth != null)
        {
            targetTowerHealth.TakeDamage(damagePerAttack);
        }
    }

    public void KillEnemy()
    {
        if (isDead) return; 
        isDead = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        gameObject.tag = "Untagged";

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool(attackBoolName, false);
            animator.SetTrigger(deathTriggerName);
        }

        Destroy(gameObject, despawnDelay);
    }
}