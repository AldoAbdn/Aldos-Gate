using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int currentRound = 1;
    private int currentPlayer = 1;

    public int Players = 2;
    // Add references to UI elements to display current round and player
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI PlayerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RoundText.text = "Round: " + currentRound;
        PlayerText.text = "Player: " + currentPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextPlayer()
    {
        currentPlayer++;
        PlayerText.text = "Player: " + currentPlayer;
        if (currentPlayer > Players)
        {
            currentPlayer = 1;
            PlayerText.text = "Player: " + currentPlayer;
            NextRound();
        }
    }

    public void NextRound()
    {
        currentRound++;
        RoundText.text = "Round: " + currentRound;
        if (currentRound > 3)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Implement game over logic here
    }
}
