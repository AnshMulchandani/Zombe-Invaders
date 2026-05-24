using UnityEngine;

public class Tower : MonoBehaviour
{
    public float maxHealth = 500f;
    public float currentHealth;

    public float maxShield = 500f;
    public float currentShield = 0f;

    public TowerHealth healthBar;  
    public TowerHealth shieldBar;  

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
        currentShield = Mathf.Min(currentShield, maxShield); // Cap it at maximum capacity

        if (shieldBar != null)
        {
            shieldBar.SetHealth(currentShield);
        }

    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;
        //Damage shields first
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

            if (currentHealth <= 0)
            {
                TowerDestroyed();
            }
        }
    }

    private void TowerDestroyed()
    {
        Debug.Log($"{gameObject.name} has fallen!");
        gameObject.SetActive(false); 
    }
}