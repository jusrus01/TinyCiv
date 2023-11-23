using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Client;

public record ChangeGameModeClientEvent(GameModeType GameModeType) : ClientEvent;
