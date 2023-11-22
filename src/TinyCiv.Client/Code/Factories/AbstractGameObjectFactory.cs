using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TinyCiv.Client.Code.UnitBuilder;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Factories
{
    public abstract class AbstractGameObjectFactory
    {
        protected UnitDirector unitDirector = new UnitDirector();
        public abstract GameObject CreateGameObject(ServerGameObject serverGameObject);
        public abstract GameObject CreateObjectDecoy(GameObjectType type, Position position);

        public static Image getGameObjectImage(TeamColor color, GameObjectType type)
        {
            switch (color)
            {
                case TeamColor.Red:
                    RedGameObjectFactory.LoadImage(type);
                    return RedGameObjectFactory.images[type];
                case TeamColor.Green:
                    GreenGameObjectFactory.LoadImage(type);
                    return GreenGameObjectFactory.images[type];
                case TeamColor.Yellow:
                    YellowGameObjectFactory.LoadImage(type);
                    return YellowGameObjectFactory.images[type];
                case TeamColor.Purple:
                    PurpleGameObjectFactory.LoadImage(type);
                    return PurpleGameObjectFactory.images[type];
                default:
                    return null;
            }
        }
    }
}
