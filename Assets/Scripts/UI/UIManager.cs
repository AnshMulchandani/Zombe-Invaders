using System.Collections; // Required for Coroutines
using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes
using TMPro; 

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI roundText;

    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;

    public FireBehaviour player1;
    public FireBehaviour player2;
    public SpawnZombies waveManager;

    // Flag to ensure the countdown only starts once
    private bool isTransitioning = false;

    void Start()
    {
        if (winText != null) winText.gameObject.SetActive(false);
        if (loseText != null) loseText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. Update UI Elements
        if (player1 != null && player1ScoreText != null)
            player1ScoreText.text = $"Player 1: {player1.playerScore}";

        if (player2 != null && player2ScoreText != null)
            player2ScoreText.text = $"Player 2: {player2.playerScore}";

        if (waveManager != null && roundText != null)
        {
            int displayRound = waveManager.CurrentRound + 1;
            if (displayRound > waveManager.TotalRounds)
                displayRound = waveManager.TotalRounds;

            roundText.text = $"Round: {displayRound} / {waveManager.TotalRounds}";
        }

        // 2. Check for End Game Conditions
        if (waveManager != null && !isTransitioning)
        {
            if (waveManager.GameLost)
            {
                StartCoroutine(EndGameRoutine(false));
            }
            else if (waveManager.GameWon)
            {
                StartCoroutine(EndGameRoutine(true));
            }
        }
    }

    // Coroutine to handle the delay and scene change
    private IEnumerator EndGameRoutine(bool hasWon)
    {
        isTransitioning = true; // Lock out the Update loop from calling this again

        // Show the correct UI text
        if (hasWon)
        {
            if (winText != null)
            {
                winText.text = waveManager.WinnerText;
                winText.gameObject.SetActive(true);
            }
        }
        else
        {
            if (loseText != null)
            {
                loseText.gameObject.SetActive(true);
            }
        }

        // Wait for exactly 10 seconds
        yield return new WaitForSeconds(10f);

        // Load the new scene
        SceneManager.LoadScene("TitleScreen");
    }
}