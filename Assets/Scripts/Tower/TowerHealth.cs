using UnityEngine;
using UnityEngine.UI; 

public class TowerHealth : MonoBehaviour
{
    public Slider slider;
    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }
    public void SetHealth(float currentHealth)
    {
        slider.value = currentHealth;
    }
}