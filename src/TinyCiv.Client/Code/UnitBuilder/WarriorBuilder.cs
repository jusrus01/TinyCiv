using System;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.UnitBuilder
{
    public class WarriorBuilder : IUnitBuilder
    {
        private Warrior warrior;

        public WarriorBuilder()
        {
            warrior = new Warrior();
        }

        public Unit Build()
        {
            return warrior;
        }

        public IUnitBuilder SetDamage(int damage)
        {
            warrior.Damage = damage;
            return this;
        }

        public IUnitBuilder SetDescription(string description)
        {
           warrior.Description = description;
            return this;
        }

        public IUnitBuilder SetExpReward(int expReward)
        {
            warrior.ExpReward = expReward;
            return this;
        }

        public IUnitBuilder SetColor(TeamColor color)
        {
            warrior.Color = color; return this;
        }

        public IUnitBuilder SetId(Guid id)
        {
            warrior.Id = id; return this;
        }

        public IUnitBuilder SetOpponentId(Guid? opponentId)
        {
            warrior.OpponentId = opponentId; return this;
        }

        public IUnitBuilder SetOwnerId(Guid ownerId)
        {
            warrior.OwnerId = ownerId; return this;
        }

        public IUnitBuilder SetPosition(Position position)
        {
            warrior.Position = position; return this;
        }

        public IUnitBuilder SetMaxHealth(int maxHealth)
        {
            warrior.MaxHealth = maxHealth; return this;
        }

        public IUnitBuilder SetProductionPrice(int productionPrice)
        {
            warrior.ProductionPrice = productionPrice; return this;
        }

        public IUnitBuilder SetSpeed(int speed)
        {
            warrior.Speed = speed; return this;
        }

        public IUnitBuilder SetBorderProperties(BorderProperties borderProperties)
        {
            warrior.Border = borderProperties;
            return this;
        }
    }
}
