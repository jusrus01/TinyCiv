using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Core.Interfaces;

public interface IGameState
{
    bool HandleEvent(ClientEvent @event);
}
