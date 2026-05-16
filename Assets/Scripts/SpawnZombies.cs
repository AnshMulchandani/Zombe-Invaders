using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class SpawnZombies : MonoBehaviour
{
    public float timeBetweenSpawns = 2f; 
    public float delayBetweenRounds = 5f;
    public int[] zombiesPerRound = new int[5] { 5, 10, 15, 20, 25 }; 

    public GameObject zombiePrefab;
    public Tower targetA;
    public Tower targetB;
    public FireBehaviour[] players;

    [System.Serializable] 
    public class SpawnZone
    {
        public string name;
        public Transform cornerA;
        public Transform cornerB;
    }

    public List<SpawnZone> spawnZones; 

    private int currentRound = 0;
    private bool isSpawningRound = false;
    private bool isGameOver = false;
    
    private List<ZombieAI> activeZombies = new List<ZombieAI>();

    void Start()
    {
        if (spawnZones.Count > 0)
        {
            StartCoroutine(StartNextRound());
        }
    }

    void Update()
    {
        if (isGameOver) return;

        CheckLossCondition();

        if (isGameOver) return;

        activeZombies.RemoveAll(zombie => zombie == null || zombie.IsDead);

        if (!isSpawningRound && activeZombies.Count == 0)
        {
            currentRound++;

            if (currentRound >= zombiesPerRound.Length) 
            {
                WinGame();
            }
            else
            {
                StartCoroutine(StartNextRound());
            }
        }
    }
    
    private IEnumerator StartNextRound()
    {
        isSpawningRound = true;

        yield return new WaitForSeconds(delayBetweenRounds);

        int zombiesToSpawn = zombiesPerRound[currentRound];
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            if (isGameOver) yield break;

            SpawnZombie();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawningRound = false;
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
        ZombieAI aiScript = newZombie.GetComponent<ZombieAI>();
        
        if (aiScript != null)
        {
            activeZombies.Add(aiScript);
            
            Transform closerTarget = GetCloserAliveTarget(spawnPosition);
            if (closerTarget != null)
            {
                aiScript.SetTarget(closerTarget);
            }
        }
    }

    private Transform GetCloserAliveTarget(Vector3 position)
    {
        bool aAlive = targetA != null && targetA.gameObject.activeInHierarchy;
        bool bAlive = targetB != null && targetB.gameObject.activeInHierarchy;

        if (aAlive && bAlive)
        {
            float distA = (position - targetA.transform.position).sqrMagnitude;
            float distB = (position - targetB.transform.position).sqrMagnitude;
            return distA < distB ? targetA.transform : targetB.transform;
        }
        else if (aAlive)
        {
            return targetA.transform;
        }
        else if (bAlive)
        {
            return targetB.transform;
        }

        return null; 
    }

    private void CheckLossCondition()
    {
        bool aAlive = targetA != null && targetA.gameObject.activeInHierarchy;
        bool bAlive = targetB != null && targetB.gameObject.activeInHierarchy;

        if (!aAlive && !bAlive)
        {
            isGameOver = true;
        }
    }

    private void WinGame()
    {
        isGameOver = true;

        FireBehaviour winner = null;
        int highestScore = -1;

        foreach (FireBehaviour player in players)
        {
            if (player != null && player.playerScore > highestScore)
            {
                highestScore = player.playerScore;
                winner = player;
            }
        }

        if (winner != null)
        {
            Debug.Log($"WINNER: {winner.gameObject.name} won with {highestScore} points!");
        }
    }
}