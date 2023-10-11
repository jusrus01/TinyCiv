﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Factories
{
    internal class GreenGameObjectFactory : AbstractGameObjectFactory
    {
        public static Dictionary<GameObjectType, string> sources = new Dictionary<GameObjectType, string> 
        {
            { GameObjectType.Warrior, "/Assets/warriorGreen.png" },
            { GameObjectType.Colonist, "/Assets/colonistGreen.png" },
            { GameObjectType.Cavalry, "/Assets/cavalryGreen.png" },
            { GameObjectType.Tarran, "/Assets/tarranGreen.png" },
            { GameObjectType.City, "/Assets/cityGreen.png" },
            { GameObjectType.Farm, "/Assets/farmGreen.png" },
            { GameObjectType.Mine, "/Assets/mineGreen.png" },
            { GameObjectType.Blacksmith, "/Assets/blacksmithGreen.png" },
            { GameObjectType.Shop, "/Assets/marketGreen.png" },
            { GameObjectType.Bank, "/Assets/bankGreen.png" },
            { GameObjectType.Port, "/Assets/portGreen.png" },
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
                    var city = new GameObject(serverGameObject);
                    city.ImageSource = sources[serverGameObject.Type];
                    return city;
                case GameObjectType.Farm:
                    var farm = new GameObject(serverGameObject);
                    farm.ImageSource = sources[serverGameObject.Type];
                    return farm;
                case GameObjectType.Mine:
                    var mine = new GameObject(serverGameObject);
                    mine.ImageSource = sources[serverGameObject.Type];
                    return mine;
                case GameObjectType.Blacksmith:
                    var blacksmith = new GameObject(serverGameObject);
                    blacksmith.ImageSource = sources[serverGameObject.Type];
                    return blacksmith;
                case GameObjectType.Shop:
                    var market = new GameObject(serverGameObject);
                    market.ImageSource = sources[serverGameObject.Type];
                    return market;
                case GameObjectType.Bank:
                    var bank = new GameObject(serverGameObject);
                    bank.ImageSource = sources[serverGameObject.Type];
                    return bank;
                case GameObjectType.Port:
                    var port = new GameObject(serverGameObject);
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
