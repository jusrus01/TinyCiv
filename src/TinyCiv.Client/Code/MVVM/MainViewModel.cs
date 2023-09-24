using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM
{
    public class MainViewModel
    {
        public ObservableValue<TeamColor> PlayerColor { get; } = new ObservableValue<TeamColor>();
        public ObservableValue<String> IsUnitStatVisible { get; } = new ObservableValue<string>("Hidden");
        public ObservableValue<String> UnitName { get; } = new ObservableValue<string>();

        public MainViewModel()
        {
        }

    }
}
