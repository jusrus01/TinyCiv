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
            { GameObjectType.Warrior, LoadImage("Assets/WarriorR.png") }
        };

        private static Dictionary<GameObjectType, ImageSource> playerGreenSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/WarriorG.png") }
        };

        private static Dictionary<GameObjectType, ImageSource> playerBlueSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/WarriorB.png") }
        };

        private static Dictionary<GameObjectType, ImageSource> playerWhiteSources = new()
        {
            { GameObjectType.Warrior, LoadImage("Assets/WarriorB.png") }
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
                PlayerColor.Blue => playerBlueSources[unitType],
                PlayerColor.White => playerWhiteSources[unitType],
                _ => null
            };
        }

        public static ImageSource GetImage(GameObject gameObject)
        {
            return GetImage(gameObject.Color, gameObject.Type);
        }
    }
}
