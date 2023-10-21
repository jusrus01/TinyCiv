namespace TinyCiv.Shared.Game;

public class ServerPosition : IEquatable<ServerPosition>
{
    public int X { get; set; }
    public int Y { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is ServerPosition other)
        {
            return other.X == X && other.Y == Y;
        }
        return false;
    }

    public bool Equals(ServerPosition? other)
    {
        if (other is null)
        {
            return false;
        }
        return other.X == X && other.Y == Y;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }

    public static bool operator ==(ServerPosition? left, ServerPosition right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(ServerPosition left, ServerPosition right)
    {
        return !(left == right);
    }
}
