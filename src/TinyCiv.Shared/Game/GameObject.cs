namespace TinyCiv.Shared.Game;

public class GameObject
{
    public GameObjectType Type { get; init; }
    public Position? Position { get; init; }
    public Guid? OwnerPlayerId { get; init; }
    public Guid Id { get; init; }
}