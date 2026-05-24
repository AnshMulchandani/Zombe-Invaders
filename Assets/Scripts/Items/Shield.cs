using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Shield Configuration")]
    public float shieldBonusAmount = 100f; 
    public void ActivateShield()
    {
        Tower[] towers = Object.FindObjectsOfType<Tower>();
        
        foreach (Tower tower in towers)
        {
            if (tower != null && tower.gameObject.activeInHierarchy)
            {
                tower.AddShield(shieldBonusAmount);
            }
        }
    }
}