namespace TinyCiv.Shared.Game;

public class Player
{
    public Guid Id { get; init; }
    public TeamColor Color { get; set; }

    /// <summary>
    /// Mainly used in server, to identify which player is associated with which connection
    /// </summary>
    public string? ConnectionId { get; init; }
}