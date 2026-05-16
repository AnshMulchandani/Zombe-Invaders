using UnityEngine;
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

    void Start()
    {
        if (winText != null) winText.gameObject.SetActive(false);
        if (loseText != null) loseText.gameObject.SetActive(false);
    }

    void Update()
    {
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

        if (waveManager != null)
        {
            if (waveManager.GameLost)
            {
                if (loseText != null && !loseText.gameObject.activeSelf)
                {
                    loseText.gameObject.SetActive(true);
                }
            }
            else if (waveManager.GameWon)
            {
                if (winText != null && !winText.gameObject.activeSelf)
                {
                    winText.text = waveManager.WinnerText;
                    winText.gameObject.SetActive(true);
                }
            }
        }
    }
}