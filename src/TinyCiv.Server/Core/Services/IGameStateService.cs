using TinyCiv.Server.Core.Interfaces;

namespace TinyCiv.Server.Core.Services;

public interface IGameStateService
{
    IGameState GetState();
    bool SetState(Guid playerId, IGameState gameState);
    void ResetState(Guid playerId);
    void SetStateInstant(IGameState gameState);
}
