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

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class GameViewModel : ObservableObject
    {
        public GameGrid gameGrid { get; private set; }

        public void GameStart(GameStartServerEvent response)
        {
            gameGrid = new GameGrid(Constants.Game.HeightSquareCount, Constants.Game.WidthSquareCount);

            gameGrid.GameObjects = response.Map.Objects
                .Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => new Warrior(serverGameObect))
                .ToList<GameObject>();
            gameGrid.Update();

            OnPropertyChanged();
        }
    }
}
