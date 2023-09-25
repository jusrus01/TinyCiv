using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Server;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class GameViewModel : ObservableObject
    {
        public GameState gameGrid { get; private set; }


        public string[] MapList { 
            get
            {
                if (gameGrid == null)
                    return new string[0];
                return gameGrid.mapImages.ToArray();
            } 
        }

        public GameObject[] GameObjectList
        {
            get
            {
                if (gameGrid == null)
                {
                    return new GameObject[0];
                }
                return gameGrid.GameObjects.ToArray();
            }
        }

        public void GameStart(GameStartServerEvent response)
        {
            gameGrid = new GameState(Constants.Game.HeightSquareCount, Constants.Game.WidthSquareCount);
            gameGrid.onPropertyChanged = () => { OnPropertyChanged("GameObjectList"); };

            var goFactory = new GameObjectFactory();

            gameGrid.GameObjects = response.Map.Objects
                //.Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => goFactory.Create(serverGameObect))
                .ToList<GameObject>();
            gameGrid.AddClickEvents();

            OnPropertyChanged("GameObjectList");
            OnPropertyChanged("MapList");
        }
    }
}
