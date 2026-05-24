using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public SpawnZombies gameManager;

    [System.Serializable]
    public class SpawnableItem
    {
        public string name;           // For easy identification in the Inspector
        public GameObject itemPrefab; // The item prefab (Grenade, Medkit, Ammo, etc.)
        public float spawnWeight;     // Higher weight = higher chance relative to others
    }

    [Header("Spawn Settings")]
    [Range(0f, 100f)]
    public float globalSpawnChancePerSecond = 30f; // e.g., 30% chance every second to spawn something
    
    public List<SpawnableItem> spawnableItems = new List<SpawnableItem>();

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
            // Wait exactly 1 second before attempting the next roll
            yield return new WaitForSeconds(1f);

            // Roll for the overall chance of spawning any item
            if (Random.Range(0f, 100f) <= globalSpawnChancePerSecond)
            {
                SpawnItem();
            }
        }
    }

    public void SpawnItem()
    {
        if (spawnableItems == null || spawnableItems.Count == 0) return;

        // 1. Calculate the total weight of all items combined
        float totalWeight = 0f;
        foreach (var item in spawnableItems)
        {
            if (item.itemPrefab != null)
            {
                totalWeight += item.spawnWeight;
            }
        }

        if (totalWeight <= 0f) return;

        // 2. Pick a random number between 0 and the total weight
        float randomWeightRoll = Random.Range(0f, totalWeight);
        float currentWeightSum = 0f;
        GameObject selectedPrefab = null;

        // 3. Determine which item corresponds to the rolled weight
        foreach (var item in spawnableItems)
        {
            if (item.itemPrefab == null) continue;

            currentWeightSum += item.spawnWeight;
            if (randomWeightRoll <= currentWeightSum)
            {
                selectedPrefab = item.itemPrefab;
                break; // Found our item, exit the loop
            }
        }

        if (selectedPrefab == null) return;

        // 4. Calculate spawn boundaries
        float minX = Mathf.Min(itemSpawn.cornerA.position.x, itemSpawn.cornerB.position.x);
        float maxX = Mathf.Max(itemSpawn.cornerA.position.x, itemSpawn.cornerB.position.x);
        float minZ = Mathf.Min(itemSpawn.cornerA.position.z, itemSpawn.cornerB.position.z);
        float maxZ = Mathf.Max(itemSpawn.cornerA.position.z, itemSpawn.cornerB.position.z);

        // FIXED: Explicitly added 'UnityEngine.' prefix to completely eliminate the CS0104 ambiguity error
        UnityEngine.Vector3 spawnPosition = new UnityEngine.Vector3(Random.Range(minX, maxX), itemSpawn.cornerA.position.y, Random.Range(minZ, maxZ));

        // FIXED: Used the selected prefab's rotation directly
        Instantiate(selectedPrefab, spawnPosition, selectedPrefab.transform.rotation);
    }
}