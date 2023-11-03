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
        private readonly GameObjectType unitType;
        private readonly int goldPrice;

        public CreateUnitCommand(Guid playerId, int row, int column, UnitModel unitModel)
        {
            this.playerId = playerId;
            this.row = row;
            this.column = column;
            unitType = unitModel.Type;
            goldPrice = unitModel.GoldPrice;
        }

        public async void Execute()
        {           
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateUnitClientEvent(playerId, row, column, unitType));
        }

        public bool CanExecute()
        {
            return CurrentPlayer.Instance.Resources.Gold >= goldPrice;
        }
    }
}
