using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public GameObject createMapTile(ServerGameObject serverGameObject)
        {
            var gameObject = GameObject.fromServerGameObject(serverGameObject);
            gameObject.ImageSource = TileSprite.GetValueOrDefault(serverGameObject.Type);
            return gameObject;
        }

        public string getMapImage(GameObjectType type)
        {
            return TileSprite.GetValueOrDefault(type);
        }
    }
}
