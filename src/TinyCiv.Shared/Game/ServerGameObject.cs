namespace TinyCiv.Shared.Game;

public class ServerGameObject
{
    public GameObjectType Type { get; init; }
    public ServerPosition? Position { get; init; }
    public Guid OwnerPlayerId { get; init; }
    public Guid Id { get; init; }
    public PlayerColor Color { get; init; }
}