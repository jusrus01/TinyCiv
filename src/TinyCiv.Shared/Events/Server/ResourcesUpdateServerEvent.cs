using TinyCiv.Shared.Game;

namespace TinyCiv.Shared.Events.Server;

public record ResourcesUpdateServerEvent(Resources Resources) : ServerEvent;
