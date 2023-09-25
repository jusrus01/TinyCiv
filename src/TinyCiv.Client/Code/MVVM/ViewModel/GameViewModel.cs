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
        public GameState gameState { get; private set; }
        public UnitMenuViewModel UnitMenuVM 
        {
            get { return gameState.UnitMenuVM; }
            set 
            { 
                gameState.UnitMenuVM = value;
            }
        }


        public string[] MapList { 
            get
            {
                if (gameState == null)
                    return new string[0];
                return gameState.mapImages.ToArray();
            } 
        }

        public GameObject[] GameObjectList
        {
            get
            {
                if (gameState == null)
                {
                    return new GameObject[0];
                }
                return gameState.GameObjects.ToArray();
            }
        }

        public void GameStart(GameStartServerEvent response)
        {
            gameState = new GameState(Constants.Game.HeightSquareCount, Constants.Game.WidthSquareCount);
            gameState.onPropertyChanged = () => { OnPropertyChanged("GameObjectList"); };

            var goFactory = new GameObjectFactory();

            gameState.GameObjects = response.Map.Objects
                //.Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => goFactory.Create(serverGameObect))
                .ToList<GameObject>();
            gameState.AddClickEvents();

            OnPropertyChanged("GameObjectList");
            OnPropertyChanged("MapList");
        }
    }
}
