﻿using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings;

public class Farm : IBuilding
{
    public int Price { get; }
    public BuildingType BuildingType { get; }
    public GameObjectType TileType { get; }
    public int IntervalMs { get; set; }

    public Farm()
    {
        Price = Constants.Game.FarmPrice;
        BuildingType = BuildingType.Farm;
        TileType = GameObjectType.Empty;
        IntervalMs = Constants.Game.FarmInterval;
    }

    public void Trigger(Guid playerId, IResourceService resourceService)
    {
        resourceService.AddResources(playerId, ResourceType.Food, 2);
        Console.WriteLine($"Building \"{GetType()}\" generated 2 Food for player: {playerId}");
    }
}