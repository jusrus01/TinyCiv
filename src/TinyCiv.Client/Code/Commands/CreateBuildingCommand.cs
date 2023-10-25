using System;
using System.Threading;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Client.Code.MVVM.Model;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Commands
{
    public class CreateBuildingCommand : IGameCommand
    {
        private readonly Guid playerId;
        private readonly ServerPosition serverPosition;
        private readonly GameObjectType buildingType;
        private readonly int industryCost;

        public CreateBuildingCommand(Guid playerId, BuildingModel buildingModel, ServerPosition serverPosition)
        {
            this.playerId = playerId;
            this.serverPosition = serverPosition;
            buildingType = buildingModel.Type;
            industryCost = buildingModel.IndustryCost;
        }

        public async void Execute()
        {
            BuildingType parsedType = (BuildingType)Enum.Parse(typeof(BuildingType), buildingType.ToString());
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateBuildingClientEvent(playerId, parsedType, serverPosition));
        }

        public bool CanExecute()
        {
            return CurrentPlayer.Instance.Resources.Industry >= industryCost;
        }
    }
}
