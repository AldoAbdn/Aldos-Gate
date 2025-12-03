using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public GameManager GameManager;
    public string PlayerName;
    public Piece[] Pieces;
    public abstract string Objective { get; }
}
