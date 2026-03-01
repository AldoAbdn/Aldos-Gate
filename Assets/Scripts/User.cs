using UnityEngine;

public class User : Player
{
    public User(string name) : base(name)
    {
    }
    public override string Objective => "You win by reaching the end tile first";
}
