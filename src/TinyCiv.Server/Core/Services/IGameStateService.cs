using TinyCiv.Server.Core.Interfaces;

namespace TinyCiv.Server.Core.Services;

public interface IGameStateService
{
    IGameState GetState();
    bool SetState(IGameState gameState);
    void ResetState();
}
