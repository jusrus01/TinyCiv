using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UnitMenuViewModel : ObservableObject
    {
        public ObservableValue<String> UnitName { get; } = new ObservableValue<string>("EMPTY");

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
