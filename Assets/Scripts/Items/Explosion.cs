using UnityEngine;
using System.Collections.Generic;
public class Explosion : MonoBehaviour
{
    public GameObject parentGrenade;
    public List<GameObject> zombieList = new List<GameObject>();
    private int score;
    public int detonate()
    {
        score = 0;
        foreach (GameObject zombie in zombieList)
        {
            if (zombie != null) 
            {
                ZombieAI enemyScript = zombie.GetComponent<ZombieAI>();
                if (enemyScript != null && !enemyScript.IsDead)
                {
                    enemyScript.KillEnemy();
                    score += enemyScript.pointsPerKill;
                }
            }
        }
        if (SoundManager.Instance != null && SoundManager.Instance.explosionSound!= null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.explosionSound);
        }
        return score;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            zombieList.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            zombieList.Remove(other.gameObject);
        }
    }
}
