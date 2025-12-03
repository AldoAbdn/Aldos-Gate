using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Player currentPlayer;
    private int currentRound = 1;
    private GameObject[,] boardState = new GameObject[9,9];
    private RoundState roundState = RoundState.Placing;
    private IList<RoundWinner> roundWins = new List<RoundWinner>();

    public GameObject Board;
    public GameObject SelectedTile;
    public GameObject SelectedPiece;
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

    public void TileClicked(GameObject tile)
    {
        SelectedTile = tile;
        if (roundState == RoundState.Placing)
        {
            Tile tileComponent = tile.GetComponent<Tile>();
            if (tileComponent is BlankTile)
            {
                return;
            }

            if (SelectedPiece != null && !tileComponent.IsOccupied)
            {
                SelectedPiece.transform.position = tile.transform.position;
                SelectedPiece.transform.position += new Vector3(0, 0, -0.1f);
                tileComponent.CurrentPiece = SelectedPiece;
                Piece pieceComponent = SelectedPiece.GetComponent<Piece>();
                pieceComponent.Flip();
                SelectedPiece = null;
                NextPlayer();
            }
        }
    }

    public void PieceClicked(GameObject piece)
    {
        if (roundState == RoundState.Placing)
        {
            Piece pieceComponent = piece.GetComponent<Piece>();
            if (pieceComponent.IsFlipped)
            {
                return;
            }

            SelectedPiece = piece;

            // TODO:Check if last piece placed (Flipping round start)
            bool lastPiecePlaced = false; // Placeholder
            if (lastPiecePlaced)
            {
                roundState = RoundState.Flipping;
            }

            // TODO:Check if any player has run out of pieces (Game Over, Anarchist wins)
            foreach (Player player in Players)
            {
                if (player.Pieces.Count() == 0)
                {
                    roundWins.Add(RoundWinner.Anarchist);
                    GameOver();
                }
            }
        }
        else if (roundState == RoundState.Flipping)
        {
            Piece pieceComponent = piece.GetComponent<Piece>();
            if (!pieceComponent.IsFlipped)
            {
                pieceComponent.Flip();

                // TODO: Check if last piece flipped, check who won round
                bool lastPieceFlipped = false; // Placeholder
                if (lastPieceFlipped)
                {
                    NextRound(RoundWinner.User); // Placeholder
                }
            }
        }
    }

    private void NextPlayer()
    {
        int currentIndex = Array.IndexOf(Players, currentPlayer);
        int nextIndex = currentIndex + 1;
        currentIndex = (currentIndex + 1) % Players.Length;
        currentPlayer = Players[currentIndex];
        PlayerText.text = "Player: " + currentPlayer.name;
    }

    private void NextRound(RoundWinner roundWinner)
    {
        roundWins.Add(roundWinner);
        currentRound++;
        roundState = RoundState.Placing;
        RoundText.text = "Round: " + currentRound;
        if (currentRound > 3)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        // TODO: Display game over screen
        if (roundWins.Contains(RoundWinner.Anarchist))
        {
            Debug.Log("Anarchist Wins!");
        }
        else if (roundWins.Count(rw => rw == RoundWinner.Agent) >= 2)
        {
            Debug.Log("Agent Wins!");
        }
        else if (roundWins.Count(rw => rw == RoundWinner.User) >= 2)
        {
            Debug.Log("User Wins!");
        } else
        {
            Debug.Log("Game Over");
        }
    }

    private GameObject GetGameObjectAtPosition(int x, int y)
    {
        return boardState[x, y];
    }

    private IList<GameObject> GetSurroundingTileGameObjects(GameObject centretile)
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

    private IList<GameObject> GetSurroundingTileGameObjects(int xCoordinate, int yCoordinate)
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

    private enum RoundState
    {
        Placing,
        Flipping,
    }

    private enum RoundWinner
    {
        Anarchist,
        Agent,
        User,
    }
}
