namespace TinyCiv.Shared.Game;

public class ServerPosition
{
    public int X { get; set; }
    public int Y { get; set; }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(ServerPosition left, ServerPosition right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(ServerPosition left, ServerPosition right)
    {
        return !(left == right);
    }
}
