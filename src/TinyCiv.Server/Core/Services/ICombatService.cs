using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface ICombatService
{
    Task InitiateCombatAsync(Guid opponentId, Func<Map, Task> combatNotifier);
}