using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameObjectFactory
    {

        public GameObjectFactory() { }

        public GameObject Create(ServerGameObject sGameObject)
        {
            switch(sGameObject.Type) 
            {
                case GameObjectType.Warrior:
                    var warrior = new Warrior(sGameObject);
                    warrior.ImageSource = Images.GetImage(warrior);
                    return warrior;
                case GameObjectType.StaticWater:
                    var water = new GameObject(sGameObject);
                    water.ImageSource = Images.GetTileImage(water.Type);
                    return water;
                case GameObjectType.StaticMountain:
                    var rock = new GameObject(sGameObject);
                    rock.ImageSource = Images.GetTileImage(rock.Type);
                    return rock;
                case GameObjectType.Empty:
                    var grass = new GameObject(sGameObject);
                    grass.ImageSource = Images.GetTileImage(grass.Type);
                    return grass;
                default:
                    var go = new GameObject(sGameObject);
                    go.ImageSource = "";
                    return go;
            }
        }
    }
}
