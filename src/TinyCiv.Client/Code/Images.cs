using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public static class Images
    {
        private static Dictionary<GameObjectType, string> playerRedSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorRed.png" },
            { GameObjectType.Colonist, "/Assets/colonistRed.png" },
            { GameObjectType.Cavalry, "/Assets/cavalryRed.png" },
            { GameObjectType.City, "/Assets/cityRed.png" },
            { GameObjectType.Farm, "/Assets/farmRed.png" },
            { GameObjectType.Mine, "/Assets/mineRed.png" },
            { GameObjectType.Blacksmith, "/Assets/blacksmithRed.png" },
            { GameObjectType.Market, "/Assets/marketRed.png" },
            { GameObjectType.Bank, "/Assets/bankRed.png" },
            { GameObjectType.Port, "/Assets/portRed.png" },
        };

        private static Dictionary<GameObjectType, string> playerGreenSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorGreen.png" },
            { GameObjectType.Colonist, "/Assets/colonistGreen.png" },
            { GameObjectType.Cavalry, "/Assets/cavalryGreen.png" },
            { GameObjectType.City, "/Assets/cityGreen.png" },
            { GameObjectType.Farm, "/Assets/farmGreen.png" },
            { GameObjectType.Mine, "/Assets/mineGreen.png" },
            { GameObjectType.Blacksmith, "/Assets/blacksmithGreen.png" },
            { GameObjectType.Market, "/Assets/marketGreen.png" },
            { GameObjectType.Bank, "/Assets/bankGreen.png" },
            { GameObjectType.Port, "/Assets/portGreen.png" },
        };

        private static Dictionary<GameObjectType, string> playerYellowSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorYellow.png" },
            { GameObjectType.Colonist, "/Assets/colonistYellow.png" },
            { GameObjectType.Cavalry, "/Assets/cavalryYellow.png" },
            { GameObjectType.City, "/Assets/cityYellow.png" },
            { GameObjectType.Farm, "/Assets/farmYellow.png" },
            { GameObjectType.Mine, "/Assets/mineYellow.png" },
            { GameObjectType.Blacksmith, "/Assets/blacksmithYellow.png" },
            { GameObjectType.Market, "/Assets/marketYellow.png" },
            { GameObjectType.Bank, "/Assets/bankYellow.png" },
            { GameObjectType.Port, "/Assets/portYellow.png" },
        };

        private static Dictionary<GameObjectType, string> playerPurpleSources = new()
        {
            { GameObjectType.Warrior, "/Assets/warriorPurple.png" },
            { GameObjectType.Colonist, "/Assets/colonistPurple.png" },
            { GameObjectType.Cavalry, "/Assets/cavalryPurple.png" },
            { GameObjectType.City, "/Assets/cityPurple.png" },
            { GameObjectType.Farm, "/Assets/farmPurple.png" },
            { GameObjectType.Mine, "/Assets/minePurple.png" },
            { GameObjectType.Blacksmith, "/Assets/blacksmithPurple.png" },
            { GameObjectType.Market, "/Assets/marketPurple.png" },
            { GameObjectType.Bank, "/Assets/bankPurple.png" },
            { GameObjectType.Port, "/Assets/portPurple.png" },
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static string GetTileImage(GameObjectType type)
        {
            return type switch
            {
                GameObjectType.StaticMountain => "/assets/rock_tile.png",
                GameObjectType.StaticWater => "/assets/water_tile.png",
                GameObjectType.Empty => "/assets/grass_tile.png",
                _ => null
            };
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
