using System;
using System.Threading;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Commands
{
    public class CreateUnitCommand : IGameCommand
    {
        private CancellationTokenSource cancellationTokenSource;
        private readonly Guid playerId;
        private readonly int row;
        private readonly int column;
        private readonly GameObjectType unitType;

        public CreateUnitCommand(Guid playerId, int row, int column, GameObjectType unitType)
        {
            this.playerId = playerId;
            this.row = row;
            this.column = column;
            this.unitType = unitType;
        }

        public async void Execute()
        {           
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateUnitClientEvent(playerId, row, column, unitType));
        }
    }
}
