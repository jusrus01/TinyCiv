namespace TinyCiv.Shared.Events.Client;

public record InterpretClientEvent(Guid PlayerId, string Content) : ClientEvent;