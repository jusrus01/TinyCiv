using TinyCiv.Client.Code.Factories;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM
{
    public class UnitModel
    {
        public int Health { get; }
        public int Damage { get; }
        public int Speed { get; }
        public int ProductionPrice { get; }
        public string Description { get; }
        public string ImagePath {  get; }
        public GameObjectType Type { get; }
        public TeamColor Color { get; }
        public string Name { get; }

        public UnitModel(int health, int damage, int speed, int productionPrice, string description, GameObjectType type, TeamColor color)
        {
            Health = health;
            Damage = damage;
            Speed = speed;
            ProductionPrice = productionPrice;
            Description = description;
            Type = type;
            Color = color;
            ImagePath = AbstractGameObjectFactory.getGameObjectImage(color, type);
            Name = type.ToString();
        }
    }
}
