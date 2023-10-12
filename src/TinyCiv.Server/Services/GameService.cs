using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Dtos.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Server.Dtos.Players;
using TinyCiv.Server.Dtos.Units;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;
using Microsoft.Extensions.Logging;
using TinyCiv.Server.Enums;

namespace TinyCiv.Server.Services;

public class GameService : IGameService
{
    private readonly ISessionService _sessionService;
    private readonly IConnectionIdAccessor _accessor;
    private readonly IResourceService _resourceService;
    private readonly IMapService _mapService;
    private readonly IInteractableObjectService _interactableObjectService;
    private readonly ICombatService _combatService;

    public GameService(ISessionService sessionService, IConnectionIdAccessor accessor, IResourceService resourceService, IMapService mapService, IInteractableObjectService interactableObjectService, ICombatService combatService)
    {
        _sessionService = sessionService;
        _accessor = accessor;
        _resourceService = resourceService;
        _mapService = mapService;
        _interactableObjectService = interactableObjectService;
        _combatService = combatService;
    }

    public ConnectPlayerResponse? ConnectPlayer()
    {
        var newPlayer = _sessionService.AddPlayer(_accessor.ConnectionId);

        if (newPlayer == null) return null;

        var resources = _resourceService.InitializeResources(newPlayer.Id);
        bool canGameStart = _sessionService.CanGameStart();

        return new ConnectPlayerResponse(newPlayer, resources, canGameStart);
    }

    public DisconnectPlayerResponse DisconnectPlayer()
    {
        _sessionService.RemovePlayerByConnectionId(_accessor.ConnectionId);

        var canGameStart = _sessionService.CanGameStart();

        // Assume other handler sent previously that game could start
        return new DisconnectPlayerResponse(canGameStart);
    }

    public Map StartGame(MapType mapType)
    {
        _sessionService.StartGame();
        var map = _mapService.Initialize(mapType) ?? throw new InvalidOperationException("Something went wrong, unable to initialize map");

        var players = _sessionService.GetPlayers();

        foreach (var player in players)
        {
            var random = new Random();
            int x = random.Next(0, Constants.Game.WidthSquareCount);
            int y = random.Next(0, Constants.Game.HeightSquareCount);
            var position = new ServerPosition { X = x, Y = y };

            while (_mapService.IsInRange(position, Constants.Game.TownSpaceFromTown, GameObjectType.Colonist) || 
                _mapService.GetUnit(position)!.Type == GameObjectType.StaticWater)
            {
                x = random.Next(0, Constants.Game.WidthSquareCount);
                y = random.Next(0, Constants.Game.HeightSquareCount);
                position = new ServerPosition { X = x, Y = y };
            }

            _mapService.CreateUnit(player.Id, position, GameObjectType.Colonist);
        }

        return map;
    }

    public CreateBuildingResponse? CreateBuilding(CreateBuildingRequest request)
    {
        bool isTownOwner = _mapService.IsTownOwner(request.PlayerId);
        bool buildingExist = BuildingsMapper.Buildings.TryGetValue(request.BuildingType, out var building);

        if (isTownOwner == false || buildingExist == false)
        {
            return null;
        }

        bool canAfford = _resourceService.GetResources(request.PlayerId).Industry >= building!.Price;

        if (canAfford == false)
        {
            return null;
        }

        var buildingTile = _mapService.CreateBuilding(request.PlayerId, request.Position, building!);

        if (buildingTile == null)
        {
            return null;
        }

        _resourceService.AddResources(request.PlayerId, ResourceType.Industry, -building.Price);
        _resourceService.AddBuilding(request.PlayerId, building!, request.ResourceUpdateCallback);

        var playerResources = _resourceService.GetResources(request.PlayerId);
        var map = _mapService.GetMap()!;

        return new CreateBuildingResponse(playerResources, map);
    }

    public Map? PlaceTown(Guid playerId)
    {
        if (_mapService.IsTownOwner(playerId))
        {
            return null;
        }

        bool result = _mapService.PlaceTown(playerId);

        if (result == false)
        {
            return null;
        }

        return _mapService.GetMap()!;
    }

    public AddUnitResponse? AddUnit(AddUnitRequest request)
    {
        if (!_mapService.IsTownOwner(request.PlayerId))
        {
            return null;
        }

        var unit = _mapService.CreateUnit(request.PlayerId, request.Position, request.UnitType);
        if (unit == null)
        {
            return null;
        }

        InteractableObjectServerEvent? interactableObjectEvent = null;
        if (unit.IsInteractable())
        {
            var interactable = _interactableObjectService.Initialize(unit);
            interactableObjectEvent = new InteractableObjectServerEvent(unit.Id, interactable.Health, interactable.AttackDamage);
        }

        var map = _mapService.GetMap()!;

        return new AddUnitResponse(unit, map, interactableObjectEvent);
    }

    public void AttackUnit(AttackUnitRequest request)
    {
        Task.Run(() => _combatService.InitiateCombatAsync(
            request.AttackerId, 
            request.OpponentId, 
            request.MapChangeNotifier, 
            request.InteractableObjectStateChangeNotifier));
    }

    public void MoveUnit(MoveUnitRequest request)
    {
        void onUnitMoved(UnitMoveResponse unitMoveResponse)
        {
            Map? map = null;

            if (unitMoveResponse == UnitMoveResponse.Moved)
            {
                map = _mapService.GetMap()!;
            }

            request.UnitMoveCallback?.Invoke(unitMoveResponse, map);
        } 

        _mapService.MoveUnitAsync(request.UnitId, request.Position, onUnitMoved);
    }
}
