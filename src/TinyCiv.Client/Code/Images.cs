using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public static class Images
    {
        private static Dictionary<GameObjectType, string> playerRedSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorRed.png" }
        };

        private static Dictionary<GameObjectType, string> playerGreenSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorGreen.png" }
        };

        private static Dictionary<GameObjectType, string> playerYellowSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorYellow.png" }
        };

        private static Dictionary<GameObjectType, string> playerPurpleSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorPurple.png" }
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static string GetImage(TeamColor color, GameObjectType unitType)
        {
            if (unitType == GameObjectType.Empty)
                return null;

            return color switch
            {
                TeamColor.Red => playerRedSources[unitType],
                TeamColor.Green => playerGreenSources[unitType],
                TeamColor.Yellow => playerYellowSources[unitType],
                TeamColor.Purple => playerPurpleSources[unitType],
                _ => null
            };
        }

        public static string GetImage(GameObject gameObject)
        {
            return GetImage(gameObject.Color, gameObject.Type);
        }
    }
}
