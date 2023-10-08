using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Factories
{
    public abstract class AbstractGameObjectFactory
    {
        public abstract GameObject CreateGameObject(ServerGameObject serverGameObject);

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
