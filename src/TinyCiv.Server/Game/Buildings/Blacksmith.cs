using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Interfaces;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings
{
    public class Blacksmith : ConstantSpeedBuilding, IVisitorElement
    {
        public Blacksmith()
        {
            Price = Constants.Game.BlacksmithPrice;
            BuildingType = BuildingType.Blacksmith;
            TileType = GameObjectType.Empty;
            IntervalMs = Constants.Game.BlacksmithInterval;
        }

        protected override void UpdateResources(Guid playerId, IResourceService resourceService)
        {
            var playerResources = resourceService.GetResources(playerId);
            if (playerResources.Gold < 1)
            {
                Console.WriteLine($"\"{GetType()}\" is not generating, because player does not have enough gold");
                return;
            }

            resourceService.AddResources(playerId, ResourceType.Industry, 5);
            resourceService.AddResources(playerId, ResourceType.Gold, -1);
            Console.WriteLine($"Building \"{GetType()}\" generated 5 Industry in exchange for 1 Gold for player: {playerId}");
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
