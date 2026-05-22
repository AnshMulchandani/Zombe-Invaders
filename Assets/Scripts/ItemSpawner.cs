using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public SpawnZombies gameManager;
    public GameObject grenadePrefab;
    public float timeBetweenSpawns;
    [System.Serializable] 
    public class ItemSpawnZone
    {
        public Transform cornerA;
        public Transform cornerB;
    }
    public ItemSpawnZone itemSpawn;
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    public IEnumerator SpawnRoutine()
    {
        while (!gameManager.isGameOver)
        {
            SpawnItem();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        
    }
    public void SpawnItem()
    {
        float minX = Mathf.Min(itemSpawn.cornerA.position.x, itemSpawn.cornerB.position.x);
        float maxX = Mathf.Max(itemSpawn.cornerA.position.x, itemSpawn.cornerB.position.x);
        float minZ = Mathf.Min(itemSpawn.cornerA.position.z, itemSpawn.cornerB.position.z);
        float maxZ = Mathf.Max(itemSpawn.cornerA.position.z, itemSpawn.cornerB.position.z);

        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), itemSpawn.cornerA.position.y, Random.Range(minZ, maxZ));

        GameObject newItem = Instantiate(grenadePrefab, spawnPosition, grenadePrefab.transform.rotation);
    }
}
