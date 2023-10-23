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
        private readonly BuildingModel buildingModel;
        private readonly ServerPosition serverPosition;

        public CreateBuildingCommand(Guid playerId, BuildingModel buildingModel, ServerPosition serverPosition)
        {
            this.playerId = playerId;
            this.buildingModel = buildingModel;
            this.serverPosition = serverPosition;
        }

        public async void Execute()
        {
            BuildingType parsedType = (BuildingType)Enum.Parse(typeof(BuildingType), buildingModel.Type.ToString());
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateBuildingClientEvent(playerId, parsedType, serverPosition));
        }

        public bool CanExecute()
        {
            return CurrentPlayer.Instance.Resources.Industry >= buildingModel.Cost;
        }
    }
}
