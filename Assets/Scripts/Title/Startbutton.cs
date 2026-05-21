using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the Player
        if (other.CompareTag("Player")) 
        {
            Debug.Log("Player touched the start button. Loading scene...");
            SceneManager.LoadScene("SampleScene"); 
        }
    }
}
