using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM
{
    public class MainViewModel : ObservableObject
    {
        private TeamColor _color;
        public TeamColor PlayerColor
        { 
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        private bool _IsUnitStatVisible;
        public string IsUnitStatVisible
        {
            get
            {
                if (_IsUnitStatVisible)
                    return "Visible";
                return "Hidden";
            }
            set
            {
                switch (value)
                {
                    case "Visible":
                        _IsUnitStatVisible = true;
                        break;
                    case "Hidden":
                        _IsUnitStatVisible = false;
                        break;
                    default:
                        _IsUnitStatVisible = false;
                        break;
                }
                OnPropertyChanged();
            }
        }

        private string _UnitName;
        public string UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                _UnitName = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {

        }

    }
}
