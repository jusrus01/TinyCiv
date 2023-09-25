using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    internal class UnitMenuViewModel : ObservableObject
    {
        public ObservableValue<String> UnitName { get; } = new ObservableValue<string>();


    }
}
