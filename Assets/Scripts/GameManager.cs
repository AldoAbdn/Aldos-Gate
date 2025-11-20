using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Player currentPlayer;
    private int currentRound = 1;
    private GameObject[,] boardState = new GameObject[9,9];

    public GameObject Board;
    public GameObject SelectedTile;
    public Player[] Players;
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI PlayerText;

    void Start()
    {
        currentPlayer = Players[0];
        RoundText.text = "Round: " + currentRound;
        PlayerText.text = "Player: " + currentPlayer.PlayerName;
        foreach (Transform child in Board.transform)
        {
            GameObject gameObject = child.gameObject;
            Tile tile = gameObject.GetComponent<Tile>();
            boardState[tile.X, tile.Y] = gameObject;
        }
    }

    public void NextPlayer()
    {
        int currentIndex = Array.IndexOf(Players, currentPlayer);
        int nextIndex = currentIndex + 1;
        if (nextIndex >= Players.Length)
        {
            NextRound();
        }
        currentIndex = (currentIndex + 1) % Players.Length;
        currentPlayer = Players[currentIndex];
        PlayerText.text = "Player: " + currentPlayer.name;
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

    public GameObject GetGameObjectAtPosition(int x, int y)
    {
        return boardState[x, y];
    }

    public IList<GameObject> GetSurroundingTileGameObjects(GameObject centretile)
    {
        Tile centerTileComponent = centretile.GetComponent<Tile>();
        List<GameObject> surroundingTiles = new List<GameObject>();
        int X = centerTileComponent.X;
        int Y = centerTileComponent.Y;
        for (int x = X - 1; x <= X + 1; x++)
        {
            for (int y = Y - 1; y <= Y + 1; y++)
            {
                if (x >= 0 && x < 9 && y >= 0 && y < 9)
                {
                    if (x != X || y != Y)
                    {
                        surroundingTiles.Add(boardState[x, y]);
                    }
                }
            }
        }
        return surroundingTiles;
    }

    public IList<GameObject> GetSurroundingTileGameObjects(int xCoordinate, int yCoordinate)
    {
        GameObject centretile = boardState[xCoordinate, yCoordinate];
        Tile centerTileComponent = centretile.GetComponent<Tile>();
        List<GameObject> surroundingTiles = new List<GameObject>();
        int X = centerTileComponent.X;
        int Y = centerTileComponent.Y;
        for (int x = X - 1; x <= X + 1; x++)
        {
            for (int y = Y - 1; y <= Y + 1; y++)
            {
                if (x >= 0 && x < 9 && y >= 0 && y < 9)
                {
                    if (x != X || y != Y)
                    {
                        surroundingTiles.Add(boardState[x, y]);
                    }
                }
            }
        }
        return surroundingTiles;
    }
}
