﻿namespace TinyCiv.Shared.Events.Server;

public record UnitStatusUpdateServerEvent(Guid UnitId, bool IsMoving, string ConnectionId) : ServerEvent;