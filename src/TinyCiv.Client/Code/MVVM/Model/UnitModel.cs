using TinyCiv.Client.Code.Factories;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.Model
{
    public class UnitModel : IBuyable
    {
        public int Health { get; }
        public int Damage { get; }
        public int Speed { get; }
        public int GoldPrice { get; }
        public string Description { get; }
        public string ImagePath { get; }
        public GameObjectType Type { get; }
        public TeamColor Color { get; }
        public string Name { get; }

        public UnitModel(int health, int damage, int speed, int price, string description, GameObjectType type, TeamColor color)
        {
            Health = health;
            Damage = damage;
            Speed = speed;
            GoldPrice = price;
            Description = description;
            Type = type;
            Color = color;
            ImagePath = AbstractGameObjectFactory.getGameObjectImage(color, type);
            Name = type.ToString();
        }

        public bool IsBuyable()
        {
            return CurrentPlayer.Instance.Resources.Gold >= GoldPrice;
        }
    }
}
