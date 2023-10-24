using System;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Client.Code.units;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.UnitBuilder
{
    public class TarranBuilder : IUnitBuilder
    {
        private Tarran tarran;

        public TarranBuilder()
        {
            tarran = new Tarran();
        }

        public IUnitBuilder SetDamage(int damage)
        {
            tarran.Damage = damage;
            return this;
        }

        public IUnitBuilder SetDescription(string description)
        {
            tarran.Description = description;
            return this;
        }

        public IUnitBuilder SetExpReward(int expReward)
        {
            tarran.ExpReward = expReward;
            return this;
        }

        public IUnitBuilder SetColor(TeamColor color)
        {
            tarran.Color = color;
            return this;
        }

        public IUnitBuilder SetId(Guid id)
        {
            tarran.Id = id;
            return this;
        }

        public IUnitBuilder SetOpponentId(Guid? opponentId)
        {
            tarran.OpponentId = opponentId;
            return this;
        }

        public IUnitBuilder SetOwnerId(Guid ownerId)
        {
            tarran.OwnerId = ownerId;
            return this;
        }

        public IUnitBuilder SetPosition(Position position)
        {
            tarran.Position = position;
            return this;
        }

        public IUnitBuilder SetMaxHealth(int maxHealth)
        {
            tarran.MaxHealth = maxHealth;
            return this;
        }

        public IUnitBuilder SetProductionPrice(int productionPrice)
        {
            tarran.ProductionPrice = productionPrice;
            return this;
        }

        public IUnitBuilder SetSpeed(int speed)
        {
            tarran.Speed = speed;
            return this;
        }

        public IUnitBuilder SetBorderProperties(BorderProperties borderProperties)
        {
            tarran.Border = borderProperties;
            return this;
        }

        public Unit Build()
        {
            return tarran;
        }
    }
}
