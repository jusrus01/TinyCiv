using System.Linq;
using System.Windows.Controls;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class GameViewModel : ObservableObject
    {
        public GameState gameState { get; private set; }
        
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

        public GameViewModel()
        {
            gameState = new GameStateProtectionProxy(
                new GameState(Constants.Game.HeightSquareCount, Constants.Game.WidthSquareCount));

            gameState.onPropertyChanged = () => { OnPropertyChanged("GameObjectList"); };
        }

        public void GameStart(GameStartServerEvent response)
        {           
            var goFactory = new MapObjectFactory();

            gameState.GameObjects = response.Map.Objects
                //.Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => goFactory.createMapTile(serverGameObect))
                .ToList<GameObject>();
            gameState.AddClickEvents();


            gameState.mapImages = response.Map.Objects
                .Select(mapImageObect => goFactory.getMapImage(mapImageObect.Type))
                .ToList();

            OnPropertyChanged("GameObjectList");
            OnPropertyChanged("MapList");
        }

    }
}
