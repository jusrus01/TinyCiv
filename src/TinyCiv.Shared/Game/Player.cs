namespace TinyCiv.Shared.Game;

public class Player
{
    public Guid Id { get; init; }
    public TeamColor Color { get; init; }

    /// <summary>
    /// Mainly used in server, to identify which player is associated with which connection
    /// </summary>
    public string? ConnectionId { get; init; }

    public override bool Equals(object? obj)
    {
        if (obj is Player other)
        {
            return Id == other.Id && Color == other.Color;
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Color.GetHashCode();
            return hash;
        }
    }
}