using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UpperMenuViewModel : ObservableObject
    {
        public ObservableValue<TeamColor> PlayerColor { get; } = new ObservableValue<TeamColor>();
        public ObservableValue<int> Gold { get; } = new ObservableValue<int>(0);

    }
}
