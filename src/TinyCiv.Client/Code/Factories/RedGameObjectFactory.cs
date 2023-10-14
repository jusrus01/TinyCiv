using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.units;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Factories
{
    internal class RedGameObjectFactory : AbstractGameObjectFactory
    {
        public static Dictionary<GameObjectType, string> sources = new Dictionary<GameObjectType, string>
        {
            { GameObjectType.Warrior, "/Assets/warriorRed.png" },
            { GameObjectType.Colonist, "/Assets/colonistRed.png" },
            { GameObjectType.Cavalry, "/Assets/cavalryRed.png" },
            { GameObjectType.Tarran, "/Assets/tarranRed.png" },
            { GameObjectType.Town, "/Assets/cityRed.png" },
            { GameObjectType.Farm, "/Assets/farmRed.png" },
            { GameObjectType.Mine, "/Assets/mineRed.png" },
            { GameObjectType.Blacksmith, "/Assets/blacksmithRed.png" },
            { GameObjectType.Shop, "/Assets/marketRed.png" },
            { GameObjectType.Bank, "/Assets/bankRed.png" },
            { GameObjectType.Port, "/Assets/portRed.png" },
        };

        public override GameObject CreateGameObject(ServerGameObject serverGameObject)
        {
            switch (serverGameObject.Type)
            {
                case GameObjectType.Warrior:
                    var warrior = new Warrior(GameObject.fromServerGameObject(serverGameObject));
                    warrior.ImageSource = sources[serverGameObject.Type];
                    return warrior;
                case GameObjectType.Colonist:
                    var colonist = new Colonist(GameObject.fromServerGameObject(serverGameObject));
                    colonist.ImageSource = sources[serverGameObject.Type];
                    return colonist;
                case GameObjectType.Cavalry:
                    var cavalry = new Cavalry(GameObject.fromServerGameObject(serverGameObject));
                    cavalry.ImageSource = sources[serverGameObject.Type];
                    return cavalry;
                case GameObjectType.Tarran:
                    var tarran = new Tarran(GameObject.fromServerGameObject(serverGameObject));
                    tarran.ImageSource = sources[serverGameObject.Type];
                    return tarran;
                case GameObjectType.Town:
                    var city = GameObject.fromServerGameObject(serverGameObject);
                    city.ImageSource = sources[serverGameObject.Type];
                    return city;
                case GameObjectType.Farm:
                    var farm = GameObject.fromServerGameObject(serverGameObject);
                    farm.ImageSource = sources[serverGameObject.Type];
                    return farm;
                case GameObjectType.Mine:
                    var mine = GameObject.fromServerGameObject(serverGameObject);   
                    mine.ImageSource = sources[serverGameObject.Type];
                    return mine;
                case GameObjectType.Blacksmith: 
                    var blacksmith = GameObject.fromServerGameObject(serverGameObject);
                    blacksmith.ImageSource = sources[serverGameObject.Type];
                    return blacksmith;
                case GameObjectType.Shop:
                    var market = GameObject.fromServerGameObject(serverGameObject);
                    market.ImageSource = sources[serverGameObject.Type];
                    return market;
                case GameObjectType.Bank:
                    var bank = GameObject.fromServerGameObject(serverGameObject);
                    bank.ImageSource = sources[serverGameObject.Type];
                    return bank;
                case GameObjectType.Port:
                    var port = GameObject.fromServerGameObject(serverGameObject);
                    port.ImageSource = sources[serverGameObject.Type];
                    return port;
                default:
                    var go = GameObject.fromServerGameObject(serverGameObject);
                    go.ImageSource = "";
                    return go;
            }
        }
    }
}
