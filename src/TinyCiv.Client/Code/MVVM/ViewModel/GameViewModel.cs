using System.Linq;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Server;

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


            gameState.mapImages = response.Map.Objects
                .Select(mapImageObect => Images.GetTileImage(mapImageObect.Type))
                .ToList();

            OnPropertyChanged("GameObjectList");
            OnPropertyChanged("MapList");
        }
    }
}
