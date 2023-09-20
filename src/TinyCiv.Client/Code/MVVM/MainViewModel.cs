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

        public MainViewModel()
        {

        }

    }
}
