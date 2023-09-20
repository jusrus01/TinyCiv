using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;

namespace TinyCiv.Client.Code.MVVM
{
    public class MainViewModel : ObservableObject
    {
        private string _serverStatus = "NULL";
        public string serverStatus { 
            get
            {
                return _serverStatus;
            }
            set
            {
                _serverStatus = value;
            }
        } 
        private int _playerCount = 0;
        public int playerCount
        {
            get
            {
                return _playerCount;
            }
            set
            {
                playerCount = value;
            }
        }

        public MainViewModel()
        {

        }

        public void join()
        {
            //serverStatus = "Connected";
           // playerCount = playerCount + 1;
            OnPropertyChanged();
        }
    }
}
