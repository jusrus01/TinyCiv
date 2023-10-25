using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.UnitBuilder;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Factories
{
    public abstract class AbstractGameObjectFactory
    {
        protected UnitDirector unitDirector = new UnitDirector();
        public abstract GameObject CreateGameObject(ServerGameObject serverGameObject);
        public abstract GameObject CreateObjectDecoy(GameObjectType type, Position position);

        public static string getGameObjectImage(TeamColor color, GameObjectType type)
        {
            switch (color)
            {
                case TeamColor.Red:
                    return RedGameObjectFactory.sources[type];
                case TeamColor.Green:
                    return GreenGameObjectFactory.sources[type];
                case TeamColor.Yellow:
                    return YellowGameObjectFactory.sources[type];
                case TeamColor.Purple:
                    return PurpleGameObjectFactory.sources[type];
                default:
                    return null;
            }
        }
    }
}
