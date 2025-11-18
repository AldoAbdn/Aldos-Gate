public class RoundTile : Tile
{
    public bool IsStart;
    public bool IsEnd => !IsStart;
    public int Round = 1;
}
