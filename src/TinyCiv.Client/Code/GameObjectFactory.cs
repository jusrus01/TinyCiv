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
                case GameObjectType.Colonist:
                    var colonist = new Warrior(sGameObject);
                    colonist.ImageSource = Images.GetImage(colonist);
                    return colonist;
                case GameObjectType.Cavalry:
                    var cavalry = new Warrior(sGameObject);
                    cavalry.ImageSource = Images.GetImage(cavalry);
                    return cavalry;
                case GameObjectType.Tarran:
                    var tarran = new Warrior(sGameObject);
                    tarran.ImageSource = Images.GetImage(tarran);
                    return tarran;
                case GameObjectType.City:
                    var city = new Warrior(sGameObject);
                    city.ImageSource = Images.GetImage(city);
                    return city;
                case GameObjectType.Farm:
                    var farm = new Warrior(sGameObject);
                    farm.ImageSource = Images.GetImage(farm);
                    return farm;
                case GameObjectType.Mine:
                    var mine = new Warrior(sGameObject);
                    mine.ImageSource = Images.GetImage(mine);
                    return mine;
                case GameObjectType.Blacksmith:
                    var blacksmith = new Warrior(sGameObject);
                    blacksmith.ImageSource = Images.GetImage(blacksmith);
                    return blacksmith;
                case GameObjectType.Market:
                    var market = new Warrior(sGameObject);
                    market.ImageSource = Images.GetImage(market);
                    return market;
                case GameObjectType.Bank:
                    var bank = new Warrior(sGameObject);
                    bank.ImageSource = Images.GetImage(bank);
                    return bank;
                case GameObjectType.Port:
                    var port = new Warrior(sGameObject);
                    port.ImageSource = Images.GetImage(port);
                    return port;
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
