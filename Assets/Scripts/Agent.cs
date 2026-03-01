public class Agent : Player
{
    public Agent(string name) : base(name)
    {
    }
    public override string Objective => "You win by stopping the user from reaching the end tile";
}
