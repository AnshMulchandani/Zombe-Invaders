using UnityEngine;

public class Tower : MonoBehaviour
{
    public float maxHealth = 500f;
    public float currentHealth;

    public float maxShield = 500f;
    public float currentShield = 0f;

    public TowerHealth healthBar;
    public TowerHealth shieldBar;

    public GameObject explosionPrefab;
    public GameObject hitDebrisPrefab;
    public float explosionScale = 1f;
    public float hitDebrisScale = 1f;
    public float explosionHeightOffset = 1.5f;
    public float hitDebrisHeightOffset = 1f;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        if (shieldBar != null)
        {
            shieldBar.SetMaxHealth(maxShield);
            shieldBar.SetHealth(currentShield);
        }
    }

    public void AddShield(float amount)
    {
        if (currentHealth <= 0) return;

        currentShield += amount;
        currentShield = Mathf.Min(currentShield, maxShield); 

        if (shieldBar != null)
        {
            shieldBar.SetHealth(currentShield);
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        
        if (currentShield > 0)
        {
            if (amount <= currentShield)
            {
                currentShield -= amount;
                amount = 0;
            }
            else
            {
                amount -= currentShield;
                currentShield = 0;
            }

            if (shieldBar != null)
            {
                shieldBar.SetHealth(currentShield);
            }

            Debug.Log($"{gameObject.name} shield absorbed damage! Remaining Shield: {currentShield}");
        }

        if (amount > 0)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);

            Debug.Log($"{gameObject.name} took {amount} damage! Current Health: {currentHealth}");

            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth);
            }

            
            SpawnHitDebris();

            if (currentHealth <= 0)
            {
                TowerDestroyed();
            }
        }
    }

    private void SpawnHitDebris()
    {
        if (hitDebrisPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * hitDebrisHeightOffset;

            GameObject debris = Instantiate(hitDebrisPrefab, spawnPosition, Quaternion.identity);
            debris.transform.localScale = Vector3.one * hitDebrisScale;

            Destroy(debris, 2f);
        }
    }

    private void TowerDestroyed()
    {
        Debug.Log($"{gameObject.name} has fallen!");

 
        if (explosionPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * explosionHeightOffset;

            GameObject explosion = Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionScale;

            Destroy(explosion, 2f);
        }

        if (SoundManager.Instance != null && SoundManager.Instance.destroyTowerSound != null)
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.destroyTowerSound);
        }

        gameObject.SetActive(false);
    }
}