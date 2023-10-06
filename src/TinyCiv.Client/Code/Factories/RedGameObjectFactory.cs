using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            { GameObjectType.City, "/Assets/cityRed.png" },
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
                    var warrior = new Warrior(serverGameObject);
                    warrior.ImageSource = sources[serverGameObject.Type];
                    return warrior;
                case GameObjectType.Colonist:
                    var colonist = new Warrior(serverGameObject);
                    colonist.ImageSource = sources[serverGameObject.Type];
                    return colonist;
                case GameObjectType.Cavalry:
                    var cavalry = new Warrior(serverGameObject);
                    cavalry.ImageSource = sources[serverGameObject.Type];
                    return cavalry;
                case GameObjectType.Tarran:
                    var tarran = new Warrior(serverGameObject);
                    tarran.ImageSource = sources[serverGameObject.Type];
                    return tarran;
                case GameObjectType.City:
                    var city = new Warrior(serverGameObject);
                    city.ImageSource = sources[serverGameObject.Type];
                    return city;
                case GameObjectType.Farm:
                    var farm = new Warrior(serverGameObject);
                    farm.ImageSource = sources[serverGameObject.Type];
                    return farm;
                case GameObjectType.Mine:
                    var mine = new Warrior(serverGameObject);   
                    mine.ImageSource = sources[serverGameObject.Type];
                    return mine;
                case GameObjectType.Blacksmith: 
                    var blacksmith = new Warrior(serverGameObject);
                    blacksmith.ImageSource = sources[serverGameObject.Type];
                    return blacksmith;
                case GameObjectType.Shop:
                    var market = new Warrior(serverGameObject);
                    market.ImageSource = sources[serverGameObject.Type];
                    return market;
                case GameObjectType.Bank:
                    var bank = new Warrior(serverGameObject);
                    bank.ImageSource = sources[serverGameObject.Type];
                    return bank;
                case GameObjectType.Port:
                    var port = new Warrior(serverGameObject);
                    port.ImageSource = sources[serverGameObject.Type];
                    return port;
                default:
                    var go = new GameObject(serverGameObject);
                    go.ImageSource = "";
                    return go;
            }
        }
    }
}
