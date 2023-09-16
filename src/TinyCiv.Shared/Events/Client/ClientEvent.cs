namespace TinyCiv.Shared.Events.Client;

/// <summary>
/// All events from client inherit from this,
/// should be consumed on the server side (client => server)
/// </summary>
public abstract record ClientEvent
{
    public string Type => GetType().Name;
}