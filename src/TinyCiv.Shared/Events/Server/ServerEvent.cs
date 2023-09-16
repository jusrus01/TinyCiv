namespace TinyCiv.Shared.Events.Server;

/// <summary>
/// All events from server inherit from this,
/// should be consumed on the client side (server => client)
/// </summary>
public abstract record ServerEvent
{
    public DateTime ExistenceStart => DateTime.UtcNow;
    public string Type => GetType().Name;
}
