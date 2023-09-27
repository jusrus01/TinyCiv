using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Client;

/// <summary>
/// Use to trigger game start
/// </summary>
public record StartGameClientEvent(MapType MapType = MapType.Test) : ClientEvent;