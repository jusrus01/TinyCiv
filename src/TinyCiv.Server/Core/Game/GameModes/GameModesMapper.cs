using TinyCiv.Server.Entities.GameStates;
using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Game.GameModes;

public class GameModesMapper
{
    public static Dictionary<GameModeType, IGameState> GameModes { get; } = new Dictionary<GameModeType, IGameState>()
    {
        { GameModeType.BuildingOnly, new OnlyBuildingState() },
        { GameModeType.UnitOnly, new OnlyUnitState() },
        { GameModeType.RestrictedMode, new RestrictedState() }
    };
}
