﻿using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings
{
    public class Shop : IBuilding
    {
        public int Price { get; }
        public BuildingType BuildingType { get; }
        public GameObjectType TileType { get; }
        public int IntervalMs { get; set; }

        public Shop()
        {
            Price = Constants.Game.ShopPrice;
            BuildingType = BuildingType.Shop;
            TileType = GameObjectType.Empty;
            IntervalMs = Constants.Game.ShopInterval;
        }

        public void Trigger(Guid playerId, IResourceService resourceService)
        {
            resourceService.AddResources(playerId, ResourceType.Gold, 2);
            Console.WriteLine($"Building \"{GetType()}\" generated 2 Gold for player: {playerId}");

            int randomValue = new Random().Next(0, 100);
            if (randomValue == 0) {
                int upgrade = IntervalMs * 10 / 100;
                IntervalMs -= upgrade;
                Console.WriteLine($"Building \"{GetType()}\" upgraded itself (-{upgrade} from interval)");
            }
        }
    }
}
