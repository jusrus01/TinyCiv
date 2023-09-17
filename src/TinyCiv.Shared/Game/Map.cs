namespace TinyCiv.Shared.Game;

public class Map
{
    // Turn this into a dictionary of <Position, GameObject>?
    // Or <Position, List<GameObject>> if more than one GameObject can be in same position.
    // For example - two units in same position when fighting?
    public List<GameObject>? Objects { get; init; }
}