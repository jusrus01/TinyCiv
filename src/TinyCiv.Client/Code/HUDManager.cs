using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Commands;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Client.Code.MVVM.Model;
using TinyCiv.Client.Code.MVVM.ViewModel;
using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code
{
    public class HUDManager
    {
        private static HUDManager _instance = new HUDManager();
        public static HUDManager Instance { 
            get
            {
                return _instance;
            }
        }

        public MainViewModel mainVM { private get; set; }
        private CityMenuViewModel cityVM;
        private ExecutionQueueViewModel executionVM;

        public void DisplayUnit(Unit unit)
        {
            if (mainVM == null)
                return;

            mainVM.LowerMenu.Value = new UnitMenuViewModel(unit);
        }

        public void DisplayCityMenu()
        {
            if (mainVM == null)
                return;

            cityVM = new CityMenuViewModel();
            mainVM.LowerMenu.Value = cityVM;
        }

        public void DisplayLobby()
        {
            if (mainVM == null)
                return;

            mainVM.LowerMenu.Value = new LobbyMenuViewModel();
        }

        public void DisplayUpperMenu()
        {
            if (mainVM == null)
                return;

            mainVM.UpperMenu.Value = new UpperMenuViewModel();
        }

        public void HideLowerMenu()
        {
            if (mainVM == null)
                return;

            mainVM.LowerMenu.Value = null;
            mainVM.GameVM.gameState.isGameObjectSelected = false;
        }

        public void DisplayExecutionQueue()
        {
            if (mainVM == null) return;

            executionVM = new ExecutionQueueViewModel();
            mainVM.ExecutionMenu.Value = executionVM;
        }

        public void FinishGameVictory()
        {
            if (mainVM == null) return;

            mainVM.Game.Value = new GameOverViewModel("VICTORY");
        }

        public void FinishGameDefeat()
        {
            if (mainVM == null) return;

            mainVM.Game.Value = new GameOverViewModel("DEFEAT");
        }

        public void AddToExecutionQueue(ClockModel clockModel)
        {
            if (executionVM == null) return;

            executionVM.AddToQueue(clockModel);
        }

        public Task AddCommandToQueue(IGameCommand createUnitCommand, int v, Position position)
        {
            return executionVM.CommandInvoker.AddCommandToQueue(createUnitCommand, v, position);
        }

        public GameState GetGameState()
        {
            return mainVM.GameVM.gameState;
        }

        public BuildingModel GetSelectedBuyBuilding()
        {
            if (cityVM == null) return null;

            return cityVM.SelectedBuyBuilding.Value;
        }

        public UnitModel GetSelectedBuyUnit()
        {
            if (cityVM == null) return null;

            return cityVM.SelectedBuyUnit.Value;
        }

        public void ExecuteBuildingPurchase(Position position)
        {
            if (cityVM == null) return;

            cityVM.ExecuteBuildingPurchase(position);
        }

        internal void ExecuteUnitPurchase(Position position)
        {
            if (cityVM == null) return;

            cityVM.ExecuteUnitPurchase(position);
        }
    }
}
