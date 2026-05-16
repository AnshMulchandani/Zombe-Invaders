using UnityEngine;
using System.Collections; 
using System.Collections.Generic;

public class SpawnZombies : MonoBehaviour
{
    public float timeBetweenSpawns = 2f; 
    public float delayBetweenRounds = 5f;
    public int[] rounds = new int[] { 5, 10, 15, 20, 25 }; 

    public GameObject zombiePrefab;
    
    public Transform targetA;
    public Transform targetB;
    
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

    public int CurrentRound => currentRound;
    public int TotalRounds => rounds.Length;
    public bool GameWon { get; private set; } = false;
    public bool GameLost { get; private set; } = false;
    public string WinnerText { get; private set; } = "";

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

            if (currentRound >= TotalRounds)
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

        if (currentRound < rounds.Length)
        {
            int zombiesToSpawn = rounds[currentRound];
            
            if (zombiesToSpawn <= 0)
            {
                zombiesToSpawn = 5;
            }

            for (int i = 0; i < zombiesToSpawn; i++)
            {
                if (isGameOver) yield break; 

                SpawnZombie();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
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
        Transform closerTarget = GetCloserTarget(spawnPosition);

        ZombieAI aiScript = newZombie.GetComponent<ZombieAI>();
        if (aiScript != null)
        {
            activeZombies.Add(aiScript);
            if (closerTarget != null)
            {
                aiScript.SetTarget(closerTarget);
            }
        }
    }

    private Transform GetCloserTarget(Vector3 position)
    {
        bool aAlive = targetA != null && targetA.gameObject.activeInHierarchy;
        bool bAlive = targetB != null && targetB.gameObject.activeInHierarchy;

        if (aAlive && bAlive)
        {
            float distA = (position - targetA.position).sqrMagnitude;
            float distB = (position - targetB.position).sqrMagnitude;
            return distA < distB ? targetA : targetB;
        }
        else if (aAlive)
        {
            return targetA;
        }
        else if (bAlive)
        {
            return targetB;
        }

        return null; 
    }

    private void CheckLossCondition()
    {
        bool aAlive = targetA != null && targetA.gameObject.activeInHierarchy;
        bool bAlive = targetB != null && targetB.gameObject.activeInHierarchy;

        if (!aAlive && !bAlive && !GameWon)
        {
            isGameOver = true;
            GameLost = true;
            Debug.Log("GAME OVER: Both towers have fallen!");
        }
    }

    private void WinGame()
    {
        isGameOver = true;
        GameWon = true;

        FireBehaviour winner = null;
        int highestScore = -1;
        bool isDraw = false;

        if (players != null)
        {
            foreach (FireBehaviour player in players)
            {
                if (player != null)
                {
                    if (player.playerScore > highestScore)
                    {
                        highestScore = player.playerScore;
                        winner = player;
                        isDraw = false;
                    }
                    else if (player.playerScore == highestScore)
                    {
                        isDraw = true;
                    }
                }
            }
        }

        if (isDraw)
        {
            WinnerText = "BOTH PLAYERS WIN";
        }
        else if (winner != null)
        {
            // Formats the text based on the GameObject's name, e.g., "PLAYER 1 WINS"
            WinnerText = $"{winner.gameObject.name.ToUpper()} WINS";
        }
    }
}