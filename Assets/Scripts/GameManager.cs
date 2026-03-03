using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Player> players = new List<Player>();
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
    public Transform Tiles;
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI PlayerText;
    public NavigationManager NavigationManager;
    public User UserPrefab;
    public Agent AgentPrefab;
    public Anarchist AnarchistPrefab;
    public Cell CellPrefab;
    public DoubleCell DoubleCellPrefab;
    public Switch SwitchPrefab;
    public Swap SwapPrefab;
    public Discard DiscardPrefab;

    void Start()
    {
        // Setup players
        InitialisePlayers();
        DistributePieces();
        // Set first player and round text
        currentPlayer = players[0];
        ShowCurrentPlayerPieces();
        RoundText.text = "Round: " + currentRound;
        PlayerText.text = "Player: " + currentPlayer.PlayerName;
        // Initialise board state
        foreach (Transform child in Board.transform)
        {
            GameObject gameObject = child.gameObject;
            Tile tile = gameObject.GetComponent<Tile>();
            boardState[tile.X, tile.Y] = gameObject;
        }
    }

    private void InitialisePlayers()
    {
        for (int i = 1; i <= 5; i++)
        {
            string playerName = PlayerPrefs.GetString($"PlayerName{i}", string.Empty);
            if (!string.IsNullOrEmpty(playerName))
            {
                switch (i)
                {
                    case 1:
                    case 4:
                        User user = Instantiate(UserPrefab);
                        user.PlayerName = playerName;
                        players.Add(user);
                        break;
                    case 2:
                    case 5:
                        Agent agent = Instantiate(AgentPrefab);
                        agent.PlayerName = playerName;
                        players.Add(agent);
                        break;
                    case 3:
                        Anarchist anarchist = Instantiate(AnarchistPrefab);
                        players.Add(anarchist);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void DistributePieces()
    {
        List<Piece> pieces = new List<Piece>();
        // Create pieces for distribution
        // 50 Pieces for 5 players, 10 pieces each
        // 48 Pices for 4 and 3 players
        // Add 16 or 18 Cell pieces
        int cellCount = players.Count == 5 ? 18 : 16;
        for (int i = 0; i < cellCount; i++)
        {
            Cell cell = Instantiate(CellPrefab, Tiles);
            cell.GameManager = this;
            pieces.Add(cell);
        }
        // Add 12 Double Cell pieces
        for (int i = 0; i < 12; i++)
        {
            DoubleCell doubleCell = Instantiate(DoubleCellPrefab, Tiles);
            doubleCell.GameManager = this;
            pieces.Add(doubleCell);
        }
        // Add 6 Switch pieces
        for (int i = 0; i < 6; i++)
        {
            Switch switchPiece = Instantiate(SwitchPrefab, Tiles);
            switchPiece.GameManager = this;
            pieces.Add(switchPiece);
        }
        // Add 6 Swap pieces
        for (int i = 0; i < 6; i++)
        {
            Swap swap = Instantiate(SwapPrefab, Tiles);
            swap.GameManager = this;
            pieces.Add(swap);
        }
        // Add 6 Discard pieces
        for (int i = 0; i < 6; i++) 
        { 
            Discard discard = Instantiate(DiscardPrefab, Tiles);
            discard.GameManager = this;
            pieces.Add(discard);
        }

        // 10 pieces per player for 5 players, 12 pieces per player for 4 and 16 pieces per player for 3 players
        int piecesPerPlayer = players.Count == 5 ? 10 : (players.Count == 4 ? 12 : 16);

        foreach (Player player in players)
        {
            for (int i = 0; i < piecesPerPlayer; i++)
            {
                if (pieces.Count == 0)
                {
                    break;
                }
                int randomIndex = Random.Range(0, pieces.Count);
                Piece piece = pieces[randomIndex];
                player.Pieces.Add(piece);
                pieces.RemoveAt(randomIndex);
            }
        }

        // Position pieces in player areas
        foreach (Player player in players)
        {
            for (int i = 0; i < player.Pieces.Count; i++)
            {
                Piece piece = player.Pieces[i];
                piece.transform.localPosition = new Vector3(0 + (0.5f*i), 0f , -i);
                piece.gameObject.SetActive(false);
            }
        }
    }

    private void ShowCurrentPlayerPieces()
    {
        foreach (Player player in players)
        {
            foreach (Piece piece in player.Pieces)
            {
                piece.gameObject.SetActive(false);
            }
        }

        foreach (Piece currentPlayerPieces in currentPlayer.Pieces)
        {
            currentPlayerPieces.gameObject.SetActive(true);
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
                    RoundTile roundTileComponent = tileComponent as RoundTile;
                    if (roundTileComponent.IsStart || roundTileComponent.IsOccupied)
                    {
                        return;
                    }
                    else if (roundTileComponent.Round == currentRound && roundTileComponent.IsEnd)
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
            foreach (Player player in players)
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
        switch (piece)
        {
            case Hack hack:
                // Hack Power: Win the round immediately
                NextRound(RoundWinner.Agent);
                break;
            case Discard discard:
                // Discard Power: Lose one piece
                break;
            case Swap swap:
                // Swap Power: Swap two pieces
                break;
            case Switch switchPiece:
                // Switch Power: Switch pieces
                break;
            case DoubleCell doubleCell:
                // Double Cell Power: Flip another piece
                ignoreNextPieceFlip = true;
                break;
            case Cell cell:
                // No power, do nothing
                break;

        }
    }

    private void NextPlayer()
    {
        int currentIndex = players.IndexOf(currentPlayer);
        int nextIndex = currentIndex + 1;
        currentIndex = (currentIndex + 1) % players.Count;
        currentPlayer = players[currentIndex];
        ShowCurrentPlayerPieces();
        PlayerText.text = "Player: " + currentPlayer.PlayerName;
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

    public void GameOver()
    {
        // TODO: Display game over screen
        if (roundWins.Contains(RoundWinner.Anarchist))
        {
            NavigationManager.NavigateToGameOverScene("Anarchist", (int)RoundWinner.Anarchist);
        }
        else if (roundWins.Count(rw => rw == RoundWinner.Agent) >= 2)
        {
            NavigationManager.NavigateToGameOverScene("Agent", (int)RoundWinner.Agent);
        }
        else if (roundWins.Count(rw => rw == RoundWinner.User) >= 2)
        {
            NavigationManager.NavigateToGameOverScene("User", (int)RoundWinner.User);
        } 
        else
        {
            NavigationManager.NavigateToGameOverScene("Draw", -1);
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

    public enum RoundState
    {
        Trading,
        Placing,
        Flipping,
    }

    public enum RoundWinner
    {
        Anarchist,
        Agent,
        User,
    }
}
