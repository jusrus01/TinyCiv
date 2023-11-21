using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Factories
{
    public class MapObjectFactory
    {
        private Dictionary<GameObjectType, string> TileSprite = new Dictionary<GameObjectType, string>
        {
            {GameObjectType.StaticMountain, "/assets/rock_tile.png"},
            {GameObjectType.StaticWater, "/assets/water_tile.png"},
            {GameObjectType.Empty, "/assets/grass_tile.png"}
        };

        public static Dictionary<GameObjectType, Image> images = new Dictionary<GameObjectType, Image>();

        public GameObject createMapTile(ServerGameObject serverGameObject)
        {
            LoadImage(serverGameObject.Type);
            var gameObject = GameObject.fromServerGameObject(serverGameObject);
            gameObject.ImageSource = images.GetValueOrDefault(serverGameObject.Type);
            return gameObject;
        }

        public void LoadImage(GameObjectType type)
        {
            if (images.ContainsKey(type)) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var image = new Image();
                image.Source = new BitmapImage(new Uri(TileSprite[type], UriKind.Relative));
                images[type] = image;
            });
        }

        public string getMapImage(GameObjectType type)
        {
            return TileSprite.GetValueOrDefault(type);
        }
    }
}
