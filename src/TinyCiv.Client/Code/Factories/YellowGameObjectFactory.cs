using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.units;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code.Structures;
using TinyCiv.Client.Code.UnitBuilder;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace TinyCiv.Client.Code.Factories
{
    internal class YellowGameObjectFactory : AbstractGameObjectFactory
    {
        public static Dictionary<GameObjectType, string> sources = new Dictionary<GameObjectType, string>
        {
            { GameObjectType.Warrior, "/Assets/warriorYellow.png" },
            { GameObjectType.Colonist, "/Assets/colonistYellow.png" },
            { GameObjectType.Cavalry, "/Assets/cavalryYellow.png" },
            { GameObjectType.Tarran, "/Assets/tarranYellow.png" },
            { GameObjectType.City, "/Assets/cityYellow.png" },
            { GameObjectType.Farm, "/Assets/farmYellow.png" },
            { GameObjectType.Mine, "/Assets/mineYellow.png" },
            { GameObjectType.Blacksmith, "/Assets/blacksmithYellow.png" },
            { GameObjectType.Shop, "/Assets/marketYellow.png" },
            { GameObjectType.Bank, "/Assets/bankYellow.png" },
            { GameObjectType.Port, "/Assets/portYellow.png" },
            { GameObjectType.Empty, "/Assets/EmptyObject.png" },
        };

        public static Dictionary<GameObjectType, Image> images = new Dictionary<GameObjectType, Image>();

        public override GameObject CreateGameObject(ServerGameObject serverGameObject)
        {
            switch (serverGameObject.Type)
            {
                case GameObjectType.Warrior:
                    LoadImage(serverGameObject.Type);
                    var warrior = new Warrior(GameObject.fromServerGameObject(serverGameObject));
                    warrior.intrinsic.Image = images[serverGameObject.Type];
                    return warrior;
                case GameObjectType.Colonist:
                    LoadImage(serverGameObject.Type);
                    var colonist = new Colonist(GameObject.fromServerGameObject(serverGameObject));
                    colonist.intrinsic.Image = images[serverGameObject.Type];
                    return colonist;
                case GameObjectType.Cavalry:
                    LoadImage(serverGameObject.Type);
                    var cavalry = new Cavalry(GameObject.fromServerGameObject(serverGameObject));
                    cavalry.intrinsic.Image = images[serverGameObject.Type];
                    return cavalry;
                case GameObjectType.Tarran:
                    LoadImage(serverGameObject.Type);
                    var tarran = new Tarran(GameObject.fromServerGameObject(serverGameObject));
                    tarran.intrinsic.Image = images[serverGameObject.Type];
                    return tarran;
                case GameObjectType.City:
                    LoadImage(serverGameObject.Type);
                    var city = new City(GameObject.fromServerGameObject(serverGameObject));
                    city.intrinsic.Image = images[serverGameObject.Type];
                    return city;
                case GameObjectType.Farm:
                    LoadImage(serverGameObject.Type);
                    var farm = GameObject.fromServerGameObject(serverGameObject);
                    farm.intrinsic.Image = images[serverGameObject.Type];
                    return farm;
                case GameObjectType.Mine:
                    LoadImage(serverGameObject.Type);
                    var mine = GameObject.fromServerGameObject(serverGameObject);
                    mine.intrinsic.Image = images[serverGameObject.Type];
                    return mine;
                case GameObjectType.Blacksmith:
                    LoadImage(serverGameObject.Type);
                    var blacksmith = GameObject.fromServerGameObject(serverGameObject);
                    blacksmith.intrinsic.Image = images[serverGameObject.Type];
                    return blacksmith;
                case GameObjectType.Shop:
                    LoadImage(serverGameObject.Type);
                    var market = GameObject.fromServerGameObject(serverGameObject);
                    market.intrinsic.Image = images[serverGameObject.Type];
                    return market;
                case GameObjectType.Bank:
                    LoadImage(serverGameObject.Type);
                    var bank = GameObject.fromServerGameObject(serverGameObject);
                    bank.intrinsic.Image = images[serverGameObject.Type];
                    return bank;
                case GameObjectType.Port:
                    LoadImage(serverGameObject.Type);
                    var port = GameObject.fromServerGameObject(serverGameObject);
                    port.intrinsic.Image = images[serverGameObject.Type];
                    return port;
                case GameObjectType.Empty:
                    LoadImage(serverGameObject.Type);
                    var empty = GameObject.fromServerGameObject(serverGameObject);
                    empty.intrinsic.Image = images[serverGameObject.Type];
                    return empty;
                default:
                    LoadImage(serverGameObject.Type);
                    var go = GameObject.fromServerGameObject(serverGameObject);
                    go.intrinsic.Image = images[GameObjectType.Empty];
                    return go;
            }
        }

        public static void LoadImage(GameObjectType type)
        {
            if (images.ContainsKey(type)) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var image = new Image();
                image.Source = new BitmapImage(new Uri(sources[type], UriKind.Relative));
                images[type] = image;
            });
        }

        public override GameObject CreateObjectDecoy(GameObjectType type, Position position)
        {
            switch (type)
            {
                case GameObjectType.Warrior:
                    LoadImage(type);
                    unitDirector.SetBuilder(new WarriorBuilder());
                    return unitDirector.ConstructUnitDecoyFor(new GameObject(TeamColor.Yellow, position, images[type]));
                case GameObjectType.Colonist:
                    LoadImage(type);
                    unitDirector.SetBuilder(new ColonistBuilder());
                    return unitDirector.ConstructUnitDecoyFor(new GameObject(TeamColor.Yellow, position, images[type]));
                case GameObjectType.Cavalry:
                    LoadImage(type);
                    unitDirector.SetBuilder(new CavalryBuilder());
                    return unitDirector.ConstructUnitDecoyFor(new GameObject(TeamColor.Yellow, position, images[type]));
                case GameObjectType.Tarran:
                    LoadImage(type);
                    unitDirector.SetBuilder(new TarranBuilder());
                    return unitDirector.ConstructUnitDecoyFor(new GameObject(TeamColor.Yellow, position, images[type]));
                default:
                    LoadImage(type);
                    GameObject gameObject = new GameObject(type, position, TeamColor.Yellow, 0.5);
                    gameObject.intrinsic.Image = images[type];
                    return gameObject;
            }
        }
    }
}
