using TinyCiv.Server.Core.Game.Buildings;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game.Buildings
{
    public class Bank : ConstantSpeedBuilding
    {
        public Bank()
        {
            Price = Constants.Game.BankPrice;
            BuildingType = BuildingType.Bank;
            TileType = GameObjectType.Empty;
            IntervalMs = Constants.Game.BankInterval;
        }

        protected override void UpdateResources(Guid playerId, IResourceService resourceService)
        {
            resourceService.AddResources(playerId, ResourceType.Gold, 5);
            Console.WriteLine($"Building \"{GetType()}\" generated 5 Gold for player: {playerId}");
        }
    }
}
