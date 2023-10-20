using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyCiv.Client.Code.Commands;
using TinyCiv.Client.Code.MVVM.Model;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class CityMenuViewModel : ObservableObject
    {
        public ObservableValue<bool> IsUnitsListVisible { get; } = new ObservableValue<bool>(true);
        public ObservableValue<bool> IsBuildingsListVisible { get; } = new ObservableValue<bool>(false);
        public ObservableValue<bool> IsUnderPurchase { get; } = new ObservableValue<bool>(false);
        public ObservableValue<UnitModel> SelectedBuyUnit { get; } = new ObservableValue<UnitModel>(null);
        public ObservableValue<BuildingModel> SelectedBuyBuilding { get; } = new ObservableValue<BuildingModel>(null);
        public RelayCommand ShowUnitsCommand => new RelayCommand(execute => ShowUnits());
        public RelayCommand ShowBuildingsCommand => new RelayCommand(execute => ShowBuildings());
        public RelayCommand SelectUnitToBuyCommand => new RelayCommand(SelectUnitToBuy, CanBuy);
        public RelayCommand SelectBuildingToBuyCommand => new RelayCommand(SelectBuildingToBuy, CanBuy);
        public RelayCommand EscapeKeyCommand => new RelayCommand(HandleEscapeKey, CanCancelPurchase);

        private void HandleEscapeKey(object obj)
        {
            SelectedBuyUnit.Value = null;
            SelectedBuyBuilding.Value = null;
            IsUnderPurchase.Value = false;
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private bool CanCancelPurchase(object parameter)
        {
            if (SelectedBuyUnit.Value != null || SelectedBuyBuilding.Value != null)
            {
                return true;
            }
            return false;
        }

        private void ShowUnits()
        {
            IsUnitsListVisible.Value = true;
            IsBuildingsListVisible.Value = false;
        }

        private void ShowBuildings()
        {
            IsBuildingsListVisible.Value = true;
            IsUnitsListVisible.Value = false;
        }

        private void SelectUnitToBuy(object parameter)
        {
            if (parameter is UnitModel unit)
            {
                SelectedBuyUnit.Value = unit;
                IsUnderPurchase.Value = true;
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        public async void ExecuteUnitPurchase(Position position)
        {
            var executionVM = HUDManager.executionVM;
            IGameCommand createUnitCommand = new CreateUnitCommand(CurrentPlayer.Id, position.row, position.column, SelectedBuyUnit.Value);
            executionVM.AddToQueue(new ClockModel(SelectedBuyUnit.Value, SelectedBuyUnit.Value.ImagePath, TimeSpan.FromMilliseconds(3500))); // exists ~500ms delay

            Mouse.OverrideCursor = Cursors.Arrow;
            IsUnderPurchase.Value = false;
            SelectedBuyUnit.Value = null;
            HUDManager.HideLowerMenu();

            await executionVM.CommandInvoker.AddCommandToQueue(createUnitCommand, 3000, position);
        }

        public async void ExecuteBuildingPurchase(Position position)
        {
            var executionVM = HUDManager.executionVM;
            ServerPosition serverPos = new ServerPosition { X = position.row, Y = position.column };

            IGameCommand createBuildingCommand = new CreateBuildingCommand(CurrentPlayer.Id, SelectedBuyBuilding.Value, serverPos);
            executionVM.AddToQueue(new ClockModel(SelectedBuyBuilding.Value, SelectedBuyBuilding.Value.ImagePath, TimeSpan.FromMilliseconds(3000))); // exists ~500ms delay

            SelectedBuyBuilding.Value = null;
            IsUnderPurchase.Value = false;
            Mouse.OverrideCursor = Cursors.Arrow;
            HUDManager.HideLowerMenu();

            await executionVM.CommandInvoker.AddCommandToQueue(createBuildingCommand, 3000, position);
        }

        private bool CanBuy(object parameter)
        {
            if (parameter is UnitModel || parameter is BuildingModel)
            {
                if (SelectedBuyUnit.Value != null || SelectedBuyBuilding.Value != null)
                {
                    return false;
                }
            }
            return true;
        }

        private void SelectBuildingToBuy(object parameter)
        {
            if (parameter is BuildingModel building)
            {
                SelectedBuyBuilding.Value = building;
                IsUnderPurchase.Value = true;
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        public List<UnitModel> UnitList
        {
            get
            {
                return new List<UnitModel>()
                    {
                        new UnitModel(10, 0, 2, 100, "Bonus: Can establish a city", GameObjectType.Colonist, CurrentPlayer.Color),
                        new UnitModel(40, 20, 2, 50, "Bonus: -", GameObjectType.Warrior, CurrentPlayer.Color),
                        new UnitModel(60, 30, 3, 100, "Bonus: -", GameObjectType.Cavalry, CurrentPlayer.Color),
                        new UnitModel(60, 10, 1, 50, "Bonus: Does 5x damage against a city and receives 5x less damage from a city",
                            GameObjectType.Tarran, CurrentPlayer.Color),
                    };
            }
        }

        public List<BuildingModel> BuildingList
        {
            get
            {
                return new List<BuildingModel>()
                {
                    new BuildingModel("+2 food", 50, CurrentPlayer.Color, GameObjectType.Farm),
                    new BuildingModel("+2 production", 50, CurrentPlayer.Color, GameObjectType.Mine),
                    new BuildingModel("+5 production, -1 gold", 100, CurrentPlayer.Color, GameObjectType.Blacksmith),
                    new BuildingModel("+2 gold", 50, CurrentPlayer.Color, GameObjectType.Shop),
                    new BuildingModel("+5 gold", 100, CurrentPlayer.Color, GameObjectType.Bank),
                    new BuildingModel("+2 production, +1 food", 100, CurrentPlayer.Color, GameObjectType.Port),
                };
            }
        }
    }
}
