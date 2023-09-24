using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Entities;

public record MovingUnit(Guid UnitId, ServerPosition TargetPosition, CancellationTokenSource CancellationTokenSource);