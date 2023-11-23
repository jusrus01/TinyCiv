﻿using TinyCiv.Server.Dtos.Buildings;
using TinyCiv.Server.Dtos.Players;
using TinyCiv.Server.Dtos.Towns;
using TinyCiv.Server.Dtos.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Services;

public interface IGameService
{
    ConnectPlayerResponse? ConnectPlayer();
    DisconnectPlayerResponse DisconnectPlayer();

    Map StartGame(MapType mapType);
    Map InitializeColonists();

    CreateBuildingResponse? CreateBuilding(CreateBuildingRequest request);
    PlaceTownResponse? PlaceTown(Guid playerId);

    AddUnitResponse? AddUnit(AddUnitRequest request);

    void AttackUnit(AttackUnitRequest request);
    void MoveUnit(MoveUnitRequest request);

    bool SetGameMode(Guid playerId, GameModeType gameMode);

    public IMapService GetMapService();
    public ISessionService GetSessionService();
    public IGameStateService GetGameStateService();
}
