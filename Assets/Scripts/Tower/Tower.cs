using UnityEngine;

public class Tower : MonoBehaviour
{
    public float maxHealth = 500f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage! Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            TowerDestroyed();
        }
    }

    private void TowerDestroyed()
    {
        Debug.Log($"{gameObject.name} has fallen!");
        
        gameObject.SetActive(false); 
    }
}