using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    public GameObject parentGrenade;
    public List<GameObject> zombieList = new List<GameObject>();
    public GameObject explosionVFX;
    public float explosionScale = 1f;
    public float explosionDestroyDelay = 3f;
    
    private int score;
    private bool hasExploded = false;

    private void OnMouseDown()
    {
        detonate(); 
    }

    public int detonate()
    {
        // Prevent double-triggering if clicked and shot at the same exact time
        if (hasExploded) return 0; 
        hasExploded = true;
        
        score = 0;

        // 1. Damage Enemies
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

        // 2. Play Audio
        if (SoundManager.Instance != null && SoundManager.Instance.explosionSound != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.explosionSound);
        }

        // 3. Spawn VFX
        if (explosionVFX != null)
        {
            // Instantiate without a parent so it isn't destroyed when the grenade disappears
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * explosionScale;
            Destroy(vfx, explosionDestroyDelay);
        }

        // 4. Destroy the Grenade
        if (parentGrenade != null)
        {
            Destroy(parentGrenade);
        }
        else
        {
            Destroy(gameObject);
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