using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void SetTarget(Transform target)
    {
        if (agent != null && target != null)
        {
            agent.SetDestination(target.position);

        }
    }
}