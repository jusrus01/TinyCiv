using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Client;

public record ChangeGameModeClientEvent(Guid PlayerId, GameModeType GameModeType) : ClientEvent;
