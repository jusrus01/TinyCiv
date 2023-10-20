using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Client.Code.MVVM.ViewModel;
using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code
{
    public static class HUDManager
    {
        public static MainViewModel mainVM;
        public static CityMenuViewModel cityVM { get; private set; }

        public static void DisplayUnit(Unit unit)
        {
            if (mainVM == null)
                return;

            mainVM.LowerMenu.Value = new UnitMenuViewModel(unit);
        }

        public static void DisplayCityMenu()
        {
            if (mainVM == null)
                return;

            cityVM = new CityMenuViewModel();
            mainVM.LowerMenu.Value = cityVM;
        }

        public static void DisplayLobby()
        {
            if (mainVM == null)
                return;

            mainVM.LowerMenu.Value = new LobbyMenuViewModel();
        }

        public static void DisplayUpperMenu()
        {
            if (mainVM == null)
                return;

            mainVM.UpperMenu.Value = new UpperMenuViewModel();
        }

        public static void HideLowerMenu()
        {
            if (mainVM == null)
                return;

            mainVM.LowerMenu.Value = null;
            mainVM.GameVM.gameState.isGameObjectSelected = false;
        }
    }
}
