using System;
using System.Collections.Generic;
using System.Windows;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UnitMenuViewModel : ObservableObject
    {
        public ObservableValue<bool> IsUnitsListVisible { get; } = new ObservableValue<bool>(true);
        public ObservableValue<bool> IsBuildingsListVisible { get; } = new ObservableValue<bool>(false);
        public ObservableValue<String> UnitName { get; } = new ObservableValue<string>("EMPTY");

        public RelayCommand ShowUnitsCommand => new RelayCommand(execute => ShowUnits());
        public RelayCommand ShowBuildingsCommand => new RelayCommand(execute => ShowBuildings());

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

        public List<UnitModel> UnitList {
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
                    new BuildingModel("+2 food", "50 prod.", CurrentPlayer.Color, GameObjectType.Farm),
                    new BuildingModel("+2 production", "50 prod.", CurrentPlayer.Color, GameObjectType.Mine),
                    new BuildingModel("+5 production, -1 gold", "100 prod.", CurrentPlayer.Color, GameObjectType.Blacksmith),
                    new BuildingModel("+2 gold", "50 prod.", CurrentPlayer.Color, GameObjectType.Market),
                    new BuildingModel("+5 gold", "100 prod.", CurrentPlayer.Color, GameObjectType.Bank),
                    new BuildingModel("+2 production, +1 food", "50 prod.", CurrentPlayer.Color, GameObjectType.Port),
                };
            }
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
