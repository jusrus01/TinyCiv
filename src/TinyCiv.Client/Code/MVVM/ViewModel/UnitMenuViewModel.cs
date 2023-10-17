using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TinyCiv.Client.Code.Commands;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Client.Code.MVVM.Model;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UnitMenuViewModel : ObservableObject
    {
        public GameState gameState;

        private CommandsManager commandsManager = new CommandsManager();
        public ObservableValue<bool> IsUnitsListVisible { get; } = new ObservableValue<bool>(true);
        public ObservableValue<bool> IsBuildingsListVisible { get; } = new ObservableValue<bool>(false);
        public ObservableValue<bool> IsUnderPurchase { get; } = new ObservableValue<bool>(false);
        public ObservableValue<UnitModel> SelectedBuyUnit { get; } = new ObservableValue<UnitModel>(null);
        public ObservableValue<BuildingModel> SelectedBuyBuilding { get; } = new ObservableValue<BuildingModel>(null);
        public ObservableValue<String> UnitName { get; } = new ObservableValue<string>("EMPTY");
        public ObservableCollection<ClockModel> ObjectsInQueue { get; } = new ObservableCollection<ClockModel>();
        public RelayCommand ShowUnitsCommand => new RelayCommand(execute => ShowUnits());
        public RelayCommand ShowBuildingsCommand => new RelayCommand(execute => ShowBuildings());
        public RelayCommand SelectUnitToBuyCommand => new RelayCommand(SelectUnitToBuy, CanBuy);
        public RelayCommand SelectBuildingToBuyCommand => new RelayCommand(SelectBuildingToBuy, CanBuy);
        public RelayCommand EscapeKeyCommand => new RelayCommand(HandleEscapeKey, CanCancelPurchase);
        public RelayCommand UndoCommand => new RelayCommand(HandleUndo, CanUndo);       
        public UnitMenuViewModel()
        {
            UpdateClocks();
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
                    new BuildingModel("+5 gold", 50, CurrentPlayer.Color, GameObjectType.Bank),
                    new BuildingModel("+2 production, +1 food", 50, CurrentPlayer.Color, GameObjectType.Port),
                };
            }
        }

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
            IGameCommand createUnitCommand = new CreateUnitCommand(CurrentPlayer.Id, position.row, position.column, SelectedBuyUnit.Value);
            ObjectsInQueue.Add(new ClockModel(SelectedBuyUnit.Value, SelectedBuyUnit.Value.ImagePath, TimeSpan.FromMilliseconds(3500))); // exists ~500ms delay

            Mouse.OverrideCursor = Cursors.Arrow;
            IsUnderPurchase.Value = false;
            SelectedBuyUnit.Value = null;

            await commandsManager.AddCommandToQueue(createUnitCommand, 3000, position);
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

        public async void ExecuteBuildingPurchase(Position position)
        {
            ServerPosition serverPos = new ServerPosition { X = position.row, Y = position.column };
            
            IGameCommand createBuildingCommand = new CreateBuildingCommand(CurrentPlayer.Id, SelectedBuyBuilding.Value, serverPos);
            ObjectsInQueue.Add(new ClockModel(SelectedBuyBuilding.Value, SelectedBuyBuilding.Value.ImagePath, TimeSpan.FromMilliseconds(3000))); // exists ~500ms delay

            SelectedBuyBuilding.Value = null;
            IsUnderPurchase.Value = false;
            Mouse.OverrideCursor = Cursors.Arrow;

            await commandsManager.AddCommandToQueue(createBuildingCommand, 3000, position);            
        }

        private void UpdateClocks()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) =>
            {
                if (ObjectsInQueue.Count > 0)
                {
                    var objectInQueue = ObjectsInQueue[0];
                    if (objectInQueue.BuyableObject.IsBuyable())
                    {
                        objectInQueue.RemainingTime = objectInQueue.RemainingTime.Subtract(TimeSpan.FromSeconds(1));
                        if (objectInQueue.RemainingTime <= TimeSpan.Zero)
                        {
                            ObjectsInQueue.Remove(objectInQueue);
                        }
                    }
                }

                CommandManager.InvalidateRequerySuggested();
            };

            timer.Start();
        }

        private bool CanUndo(object arg)
        {
            return ObjectsInQueue.Count > 0;
        }

        private void HandleUndo(object obj)
        {
            ObjectsInQueue.RemoveAt(ObjectsInQueue.Count - 1);
            Position undoPosition = commandsManager.UndoLastCommand();

            int index = gameState.PositionToIndex(undoPosition);
            gameState.DecoyObjects.Remove(index);
            var serverGameObject = new ServerGameObject
            {
                Type = GameObjectType.Empty,
                Position = new ServerPosition() { X = undoPosition.row, Y = undoPosition.column }
            };
            var redFactory = new RedGameObjectFactory();
            var tileReplacement = redFactory.CreateGameObject(serverGameObject);
            gameState.AddClickEvent(tileReplacement);
            gameState.GameObjects[index] = tileReplacement;
            gameState.onPropertyChanged?.Invoke();
        }

        public void SetCurrentUnit(GameObject gameObject)
        {
            UnitName.Value = gameObject.Type.ToString();
        }

        public void UnselectUnit()
        {
            UnitName.Value = "";
        }
    }
}
