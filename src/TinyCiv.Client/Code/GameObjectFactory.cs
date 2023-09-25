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
                case GameObjectType.Empty: 
                    var go = new GameObject(sGameObject);
                    go.ImageSource = "";
                    return go;
                case GameObjectType.Warrior:
                    var warrior = new Warrior(sGameObject);
                    warrior.ImageSource = Images.GetImage(warrior);
                    return warrior;
                default:
                    return null;
            }
        }
    }
}
