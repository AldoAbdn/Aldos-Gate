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
    private bool startTileSelected = false;
    private bool startPieceFlipped = false;
    private bool ignoreNextPieceFlip = false;

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

            // Do not allow placement on blank tiles
            if (tileComponent is BlankTile)
            {
                return;
            }

            // Starting piece placement
            if (!startTileSelected)
            {
                if (tileComponent is RoundTile)
                {
                    RoundTile roundTileComponent = tileComponent as RoundTile;
                    if (roundTileComponent.IsOccupied || roundTileComponent.IsEnd || roundTileComponent.Round != currentRound)
                    {
                        return;
                    }
                    else
                    {
                        PlaceSelectedPiece(tileComponent);
                        startTileSelected = true;
                    }
                }
                else
                {
                    return;
                }
            } 
            else
            {
                // Don't allow placement on round start tiles or occupied round tiles
                if (tileComponent is RoundTile)
                {
                    RoundTile rountTileComponent = tileComponent as RoundTile;
                    if (rountTileComponent.IsStart || rountTileComponent.IsOccupied)
                    {
                        return;
                    }
                    else if (rountTileComponent.Round == currentRound && rountTileComponent.IsEnd)
                    {
                        // Check if there is a path from the start tile to this end tile
                        if (FullPathExists())
                        {
                            // Allow placement on end round tile
                            PlaceSelectedPiece(tileComponent);
                            // Flipping round starts
                            roundState = RoundState.Flipping;
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                // Normal piece placement
                PlaceSelectedPiece(tileComponent);
            }
        }
    }

    private bool FullPathExists()
    {
        // TODO: Implement pathfinding algorithm to check for full path from start to end
        return false;
    }

    private void PlacePiece(Tile tile, GameObject piece)
    {
        if (piece != null && !tile.IsOccupied)
        {
            piece.transform.position = tile.transform.position;
            piece.transform.position += new Vector3(0, 0, -0.1f);
            tile.CurrentPiece = piece;
            Piece pieceComponent = piece.GetComponent<Piece>();
            pieceComponent.Flip();
            // Check for anarchist win condition
            foreach (Player player in Players)
            {
                if (player.Pieces.Count() == 0)
                {
                    roundWins.Add(RoundWinner.Anarchist);
                    GameOver();
                    return;
                }
            }
            NextPlayer();
        }
    }

    private void PlaceSelectedPiece(Tile tile)
    {
        PlacePiece(tile, SelectedPiece);
        SelectedPiece = null;
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
        }
        else if (roundState == RoundState.Flipping)
        {
            Piece pieceComponent = piece.GetComponent<Piece>();
            if (!pieceComponent.IsFlipped)
            {
                if (pieceComponent.Tile is RoundTile)
                {
                    RoundTile roundTileComponent = pieceComponent.Tile as RoundTile;
                    if (roundTileComponent.IsStart && !startPieceFlipped)
                    {
                        pieceComponent.Flip();
                        startPieceFlipped = true;
                    }
                    else if (roundTileComponent.IsEnd)
                    {
                        // Check for round win conditions
                        if (pieceComponent is Hack && !ignoreNextPieceFlip)
                        {
                            pieceComponent.Flip();
                            NextRound(RoundWinner.Agent);
                        }
                        else
                        {
                            pieceComponent.Flip();
                            NextRound(RoundWinner.User);
                        }
                    }
                }
                pieceComponent.Flip();
                if (ignoreNextPieceFlip)
                {
                    ignoreNextPieceFlip = false;
                    return;
                }
                ApplyPiecePower(pieceComponent);
            }
        }
    }

    private void ApplyPiecePower(Piece piece)
    {
        if (piece is Cell)
        {
            return;
        } 
        else if (piece is Hack)
        {
            NextRound(RoundWinner.Agent);
        }
        else if (piece is Discard) {
            // TODO: Discard Power: Lose one piece
            return;
        }
        else if (piece is Swap)
        {
            // TODO: Swap Power: Swap two pieces
            return;
        }
        else if (piece is Switch)
        {
            // TODO: Switch Power: Switch pieces
            return;
        }
        else if (piece is DoubleCell)
        {
            ignoreNextPieceFlip = true;
            return;
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
        startTileSelected = false;
        startPieceFlipped = false;
        ignoreNextPieceFlip = false;
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
