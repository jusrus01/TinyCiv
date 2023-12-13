using TinyCiv.Server.Entities.GameStates;
using TinyCiv.Server.Core.Game.Buildings;
﻿using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Dtos.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Server.Dtos.Players;
using TinyCiv.Server.Dtos.Towns;
using TinyCiv.Server.Dtos.Units;
using TinyCiv.Shared.Game;
using TinyCiv.Server.Enums;
using TinyCiv.Shared;
using TinyCiv.Server.Core.Game.GameModes;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Interpreter;
using TinyCiv.Server.Interpreter.Expressions;

namespace TinyCiv.Server.Services;

public class GameService : IGameService
{
    private readonly ISessionService _sessionService;
    private readonly IConnectionIdAccessor _accessor;
    private readonly IResourceService _resourceService;
    private readonly IMapService _mapService;
    private readonly IInteractableObjectService _interactableObjectService;
    private readonly ICombatService _combatService;
    private readonly IPublisher _publisher;
    private readonly ILogger<GameService> _logger;
    private readonly IGameStateService _gameStateService;

    public GameService(
        ISessionService sessionService,
        IConnectionIdAccessor accessor,
        IResourceService resourceService,
        IMapService mapService,
        IInteractableObjectService interactableObjectService,
        ICombatService combatService,
        IGameStateService gameStateService)
        IPublisher publisher,
        ILogger<GameService> logger)
    {
        _sessionService = sessionService;
        _accessor = accessor;
        _resourceService = resourceService;
        _mapService = mapService;
        _interactableObjectService = interactableObjectService;
        _combatService = combatService;
        _publisher = publisher;
        _logger = logger;
        _gameStateService = gameStateService;
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
        _gameStateService.SetStateInstant(new NormalState());
        return map;
    }

    public Map InitializeColonists()
    {
        var playerIterator = _sessionService.GetIterator();
        while (playerIterator.HasNext())
        {
            var player = playerIterator.Next();
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

        var map = _mapService.GetMap()!;

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

    public PlaceTownResponse? PlaceTown(Guid playerId)
    {
        if (_mapService.IsTownOwner(playerId))
        {
            return null;
        }

        var createdTown = _mapService.PlaceTown(playerId);

        if (createdTown == null)
        {
            return null;
        }
        
        var interactable = _interactableObjectService.Initialize(createdTown);
        var interactableInitEvent = new InteractableObjectServerEvent(createdTown.Id, interactable.Health, interactable.AttackDamage);

        return new PlaceTownResponse(_mapService.GetMap()!, interactableInitEvent);
    }

    public AddUnitResponse? AddUnit(AddUnitRequest request)
    {
        if (!_mapService.IsTownOwner(request.PlayerId))
        {
            return null;
        }

        var responseServerEvents = new List<ServerEvent>();

        var interactableStaticInfo = _interactableObjectService.GetInfo(request.UnitType);
        if (interactableStaticInfo != null)
        {
            var resourcesAfterPurchase = _resourceService.BuyInteractable(request.PlayerId, interactableStaticInfo);
            if (resourcesAfterPurchase == null)
            {
                _logger.LogWarning("Player '{player_id}' was not able to buy new unit, not enough resources", request.PlayerId);
                return null;
            }
            
            responseServerEvents.Add(new ResourcesUpdateServerEvent(resourcesAfterPurchase));
        }
        
        var unit = _mapService.CreateUnit(request.PlayerId, request.Position, request.UnitType);
        if (unit == null)
        {
            _resourceService.CancelInteractablePayment(request.PlayerId, interactableStaticInfo);
            return null;
        }

        if (interactableStaticInfo != null)
        {
            var interactable = _interactableObjectService.Initialize(unit);
            responseServerEvents.Add(new InteractableObjectServerEvent(unit.Id, interactable.Health, interactable.AttackDamage));
        }

        var map = _mapService.GetMap()!;

        return new AddUnitResponse(unit, map, responseServerEvents.ToArray());
    }

    public void AttackUnit(AttackUnitRequest request)
    {
        Task.Run(() => _combatService.InitiateCombatAsync(
            request.AttackerId, 
            request.OpponentId, 
            request.MapChangeNotifier, 
            request.InteractableObjectStateChangeNotifier,
            request.NewUnitNotifier));
    }

    public void MoveUnit(MoveUnitRequest request)
    {
        void OnUnitMoved(UnitMoveResponse unitMoveResponse)
        {
            Map? map = null;

            if (unitMoveResponse == UnitMoveResponse.Moved)
            {
                map = _mapService.GetMap()!;
            }

            request.UnitMoveCallback?.Invoke(unitMoveResponse, map);
        } 

        _mapService.MoveUnitAsync(request.UnitId, request.Position, OnUnitMoved);
    }

    public bool SetGameMode(Guid playerId, GameModeType gameModeType, Action onGamemodeReset)
    {
        var gameState = GameModesMapper.GameModes[gameModeType];
        bool success = _gameStateService.SetState(playerId, gameState);

        Task.Run(async () =>
        {
            await Task.Delay(Constants.Game.GameModeAbilityDurationMs);
            _gameStateService.ResetState(playerId);
            onGamemodeReset?.Invoke();
        });
        
        return success;
    }

    public IMapService GetMapService()
    {
        return _mapService;
    }

    public ISessionService GetSessionService()
    {
        return _sessionService;
    }

    public IGameStateService GetGameStateService()
    {
        return _gameStateService;
	}
    
    public ConditionContext? EvaluateCondition(ConditionContext lastEvaluationContext)
    {
        var latestContext = new ConditionContext
        {
            Condition = lastEvaluationContext.Condition,
            PlayerId = lastEvaluationContext.PlayerId,
        };

        if (lastEvaluationContext.Condition == Condition.None)
        {
            latestContext.Result = false;
            latestContext.UnitCount = 0;
            return latestContext;
        }

        var mapObjects = _mapService.GetMapObjects();
        var playerInteractableUnits = mapObjects
            .Where(obj => obj.OwnerPlayerId == lastEvaluationContext.PlayerId && obj.IsInteractable())
            .ToList();
        latestContext.UnitCount = playerInteractableUnits.Count;

        switch (lastEvaluationContext.Condition)
        {
            case Condition.AnyUnitsAlive:
                latestContext.Result = playerInteractableUnits.Any(unit => unit.Type != GameObjectType.City);
                break;
            case Condition.NotUnderAttack:
                latestContext.Result = playerInteractableUnits.All(unit => unit.OpponentId == null);
                break;
            case Condition.MoreThanOneUnitLeft:
                latestContext.Result = playerInteractableUnits.Count > 1;
                break;
            case Condition.AllCurrentUnitsAlive:
                latestContext.Result = playerInteractableUnits.Count >= lastEvaluationContext.UnitCount;
                break;
            default:
                return null;
        }

        return latestContext;
    }

    public void PerformMassAttackOnFirstEnemyUnit(
        Guid playerId,
        GameObjectType playerUnits,
        GameObjectType enemyUnits,
        TeamColor colorToAttack)
    {
        var mapObjects = _mapService.GetMapObjects();
        var availableUnitsForAttack = mapObjects
            .Where(obj => obj.OwnerPlayerId == playerId && obj.Type == playerUnits && obj.IsInteractable() && obj.Type != GameObjectType.City)
            .ToList();

        if (!availableUnitsForAttack.Any())
        {
            return;
        }

        var unitToAttack = mapObjects
            .FirstOrDefault(obj => obj.Type == enemyUnits && obj.IsInteractable() && obj.Color == colorToAttack);
        if (unitToAttack == null)
        {
            return;
        }
        
        foreach (var unitToMove in availableUnitsForAttack)
        {
            _mapService.MoveUnitAsync(
                unitToMove.Id,
                unitToAttack.Position!,
                OnUnitMoved);
        }
        
        // copy-paste from handlers
        // ...
        Task MapChangeNotifier(Map updatedMap) =>
            _publisher.NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(updatedMap));

        // NOTE: not used by client
        Task NewUnitNotifier(ServerGameObject gameObject) =>
            _publisher.NotifyAllAsync(Constants.Server.SendCreatedUnit, new CreateUnitServerEvent(gameObject));
        
        Task InteractableObjectStateChangeNotifier(IInteractableObject interactableObject) =>
            _publisher.NotifyAllAsync(Constants.Server.SendInteractableObjectChangesToAll, new InteractableObjectServerEvent(interactableObject.GameObjectReferenceId, interactableObject.Health, interactableObject.AttackDamage));
        
        void OnUnitMoved(UnitMoveResponse unitMoveResponse)
        {
            if (unitMoveResponse == UnitMoveResponse.Moved)
            {
                _publisher.NotifyAllAsync(Constants.Server.SendMapChangeToAll, new MapChangeServerEvent(_mapService.GetMap()!)).GetAwaiter().GetResult();
            }

            if (unitMoveResponse == UnitMoveResponse.Stopped)
            {
                foreach (var unit in availableUnitsForAttack)
                {
                    _combatService.InitiateCombatAsync(
                        unit.Id,
                        unitToAttack.Id,
                        MapChangeNotifier,
                        InteractableObjectStateChangeNotifier,
                        NewUnitNotifier);
                }
            }
        }
    }
}
