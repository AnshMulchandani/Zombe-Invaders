using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    private List<GameObject> playersOnButton = new List<GameObject>();

    public int requiredPlayersToStart = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playersOnButton.Contains(other.gameObject))
            {
                playersOnButton.Add(other.gameObject);
            }

            // Check if both players are now inside the trigger
            if (playersOnButton.Count >= requiredPlayersToStart)
            {
                SceneManager.LoadScene("SampleScene"); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If a player steps off the button, remove them from the list        if (other.CompareTag("Player"))
        {
            if (playersOnButton.Contains(other.gameObject))
            {
                playersOnButton.Remove(other.gameObject);
            }
        }
    }
}
