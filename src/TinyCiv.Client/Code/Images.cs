using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public static class Images
    {
        private static Dictionary<GameObjectType, ImageSource> playerRedSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/warriorRed.png") }
        };

        private static Dictionary<GameObjectType, ImageSource> playerGreenSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/warriorGreen.png") }
        };

        private static Dictionary<GameObjectType, ImageSource> playerYellowSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/warriorYellow.png") }
        };

        private static Dictionary<GameObjectType, ImageSource> playerPurpleSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/warriorPurple.png") }
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static ImageSource GetImage(PlayerColor color, GameObjectType unitType)
        {
            return color switch
            {
                PlayerColor.Red => playerRedSources[unitType],
                PlayerColor.Green => playerGreenSources[unitType],
                PlayerColor.Yellow => playerYellowSources[unitType],
                PlayerColor.Purple => playerPurpleSources[unitType],
                _ => null
            };
        }

        public static ImageSource GetImage(GameObject gameObject)
        {
            return GetImage(gameObject.Color, gameObject.Type);
        }
    }
}
