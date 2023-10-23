using System;
using System.Threading;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Client.Code.MVVM.Model;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Commands
{
    public class CreateUnitCommand : IGameCommand
    {
        private readonly Guid playerId;
        private readonly int row;
        private readonly int column;
        private readonly UnitModel unitModel;

        public CreateUnitCommand(Guid playerId, int row, int column, UnitModel unitModel)
        {
            this.playerId = playerId;
            this.row = row;
            this.column = column;
            this.unitModel = unitModel;
        }

        public async void Execute()
        {           
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateUnitClientEvent(playerId, row, column, unitModel.Type));
        }

        public bool CanExecute()
        {
            return CurrentPlayer.Instance.Resources.Industry >= unitModel.ProductionPrice;
        }
    }
}
