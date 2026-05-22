using UnityEngine;

public class Tower : MonoBehaviour
{
    public float maxHealth = 500f;
    public float currentHealth;

    public TowerHealth healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        
        currentHealth = Mathf.Max(currentHealth, 0);


        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            TowerDestroyed();
        }
    }

    private void TowerDestroyed()
    {        
        gameObject.SetActive(false); 
    }
}