using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    public GameObject parentGrenade;

    public List<GameObject> zombieList = new List<GameObject>();

    // NEW: visual explosion effect that appears when the grenade is selected
    public GameObject explosionVFX;

    // NEW: size of the visual explosion
    public float explosionScale = 1f;

    // NEW: time before deleting the explosion visual effect
    public float explosionDestroyDelay = 3f;

    private int score;

    // NEW: prevents the grenade from exploding more than once
    private bool hasExploded = false;

    // NEW: this happens when you click/select the grenade
    private void OnMouseDown()
    {
        ExplodeGrenade();
    }

    // NEW: controls the full grenade explosion
    private void ExplodeGrenade()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Kill the zombies inside the explosion area
        detonate();

        // Spawn the visual explosion
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            vfx.transform.localScale = Vector3.one * explosionScale;
            Destroy(vfx, explosionDestroyDelay);
        }

        // Destroy the grenade object
        if (parentGrenade != null)
        {
            Destroy(parentGrenade);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        if (SoundManager.Instance != null && SoundManager.Instance.explosionSound != null)
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