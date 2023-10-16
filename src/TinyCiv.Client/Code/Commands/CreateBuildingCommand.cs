using System;
using System.Threading;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Commands
{
    public class CreateBuildingCommand : IGameCommand
    {
        private CancellationTokenSource cancellationTokenSource;
        private readonly Guid playerId;
        private readonly BuildingType buildingType;
        private readonly ServerPosition serverPosition;

        public CreateBuildingCommand(Guid playerId, BuildingType buildingType, ServerPosition serverPosition)
        {
            this.playerId = playerId;
            this.buildingType = buildingType;
            this.serverPosition = serverPosition;
        }

        public async void Execute()
        {
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateBuildingClientEvent(playerId, buildingType, serverPosition));
        }
    }
}
