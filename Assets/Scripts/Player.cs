using UnityEngine;
using System.Collections.Generic;

public abstract class Player : MonoBehaviour
{
    public string PlayerName;
    public List<Piece> Pieces;
    public abstract string Objective { get; }
}
