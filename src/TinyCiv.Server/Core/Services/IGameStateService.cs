using TinyCiv.Server.Core.Interfaces;

namespace TinyCiv.Server.Core.Services;

public interface IGameStateService
{
    IGameState GetState();
    bool SetState(Guid PlayerId, IGameState gameState);
    void SetStateInstant(IGameState gameState);
}
