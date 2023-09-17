using TinyCiv.Shared.Dto;

namespace TinyCiv.Shared.Events.Server;

public record JoinLobbyServerEvent(PlayerDto NewPlayer) : ServerEvent;
