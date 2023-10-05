﻿using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings
{
    public class Port : IBuilding
    {
        public int Price { get; }
        public BuildingType BuildingType { get; }
        public GameObjectType TileType { get; }
        public int IntervalMs { get; set; }

        public Port()
        {
            Price = Constants.Game.PortPrice;
            BuildingType = BuildingType.Port;
            TileType = GameObjectType.StaticWater;
            IntervalMs = Constants.Game.PortInterval;
        }

        public void Trigger(Guid playerId, IResourceService resourceService)
        {
            resourceService.AddResources(playerId, ResourceType.Food, 1);
            resourceService.AddResources(playerId, ResourceType.Industry, 2);
            Console.WriteLine($"Building \"{GetType()}\" generated 1 Food and 2 Industry for player: {playerId}");

            IntervalMs = new Random().Next(2000, 7000);
            Console.WriteLine($"Building \"{GetType()}\" changed it's interval to {IntervalMs}");
        }
    }
}