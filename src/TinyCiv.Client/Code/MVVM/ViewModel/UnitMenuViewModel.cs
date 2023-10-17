using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UnitMenuViewModel : ObservableObject
    {
        public ObservableValue<string> UnitType { get; } = new ObservableValue<string>("EMPTY");
        public ObservableValue<string> ImageSource { get;  } = new ObservableValue<string>();

        public UnitMenuViewModel(Unit unit)
        {
            UnitType.Value = unit.Type.ToString();
            ImageSource.Value = unit.ImageSource.ToString();
        }
    }
}
