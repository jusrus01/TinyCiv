using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using TinyCiv.Client.Code.Commands;
using System.Windows.Media;
using TinyCiv.Client.Code.units;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UnitMenuViewModel : ObservableObject
    {
        public ObservableValue<string> UnitType { get; } = new ObservableValue<string>("EMPTY");
        public ObservableValue<string> ImageSource { get;  } = new ObservableValue<string>();
        public ObservableValue<string> IsColonist { get; } = new ObservableValue<string>("Collapsed");
        public ObservableValue<RelayCommand> SettleDownCommand { get; } = new ObservableValue<RelayCommand>();

        private Unit _unitReference;

        public UnitMenuViewModel(Unit unit)
        {
            _unitReference = unit;

            UnitType.Value = unit.Type.ToString();
            ImageSource.Value = unit.ImageSource.ToString();
            if (unit is Colonist)
            {
                var colonist = (Colonist)unit;
                IsColonist.Value = "Visible";
                SettleDownCommand.Value = new RelayCommand(o => { colonist.SettleDown(); });
            }
        }
    }
}
