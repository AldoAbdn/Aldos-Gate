using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int currentRound = 1;
    private int currentPlayer = 1;
    private GameObject[,] boardState = new GameObject[9,9];
    private Tuple<int, int> center = Tuple.Create(4, 4);

    public GameObject Board;
    public int Players = 2;
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI PlayerText;

    void Start()
    {
        RoundText.text = "Round: " + currentRound;
        PlayerText.text = "Player: " + currentPlayer;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                boardState[i, j] = Board.transform.Find($"{i},{j}").gameObject;
            }
        }
    }

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
