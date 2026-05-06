using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class SpawnZombies : MonoBehaviour
{
    public float timeBetweenSpawns = 2f; 
    public GameObject zombiePrefab;
    public Transform targetA;
    public Transform targetB;

    [System.Serializable] 
    public class SpawnZone
    {
        public string name;
        public Transform cornerA;
        public Transform cornerB;
    }

    public List<SpawnZone> spawnZones; 

    void Start()
    {
        if (spawnZones.Count > 0)
        {
            StartCoroutine(SpawnRoutine());
        }
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnZombie();
        }
    }

    public void SpawnZombie()
    {
        int randomSpawnZone = Random.Range(0, spawnZones.Count);
        SpawnZone zone = spawnZones[randomSpawnZone];

        float minX = Mathf.Min(zone.cornerA.position.x, zone.cornerB.position.x);
        float maxX = Mathf.Max(zone.cornerA.position.x, zone.cornerB.position.x);
        float minZ = Mathf.Min(zone.cornerA.position.z, zone.cornerB.position.z);
        float maxZ = Mathf.Max(zone.cornerA.position.z, zone.cornerB.position.z);

        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), zone.cornerA.position.y, Random.Range(minZ, maxZ));

        GameObject newZombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        Transform closerTarget = GetCloserTarget(spawnPosition);

        ZombieAI aiScript = newZombie.GetComponent<ZombieAI>();
        if (aiScript != null)
        {
            aiScript.SetTarget(closerTarget);
        }
    }

    private Transform GetCloserTarget(Vector3 position)
    {
        float distA = (position - targetA.position).sqrMagnitude;
        float distB = (position - targetB.position).sqrMagnitude;

        return distA < distB ? targetA : targetB;
    }
}