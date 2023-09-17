using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TinyCiv.Client.Code.units;

namespace TinyCiv.Client.Code
{
    public static class Images
    {
        private static Dictionary<GameObjectType, ImageSource> playerASources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/WarriorA.png") }
        };

        private static Dictionary<GameObjectType, ImageSource> playerBSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/WarriorB.png") }
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static ImageSource GetImage(int playerId, GameObjectType unitType)
        {
            return playerId switch
            {
                1 => playerASources[unitType],
                2 => playerBSources[unitType],
                _ => null
            };
        }

        public static ImageSource GetImage(GameObject unit)
        {
            return GetImage(unit.playerId, unit.Type);
        }
    }
}
