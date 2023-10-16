using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Dtos.Players;

public record ConnectPlayerResponse(Player Player, Resources Resources, bool CanGameStart);
