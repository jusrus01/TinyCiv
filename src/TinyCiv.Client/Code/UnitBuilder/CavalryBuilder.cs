using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.UnitBuilder
{
    public class CavalryBuilder : IUnitBuilder
    {
        private Cavalry cavalry;

        public CavalryBuilder()
        {
            cavalry = new Cavalry();
        }

        public IUnitBuilder SetDamage(int damage)
        {
            cavalry.Damage = damage;
            return this;
        }

        public IUnitBuilder SetDescription(string description)
        {
            cavalry.Description = description;
            return this;
        }

        public IUnitBuilder SetExpReward(int expReward)
        {
            cavalry.ExpReward = expReward;
            return this;
        }

        public IUnitBuilder SetColor(TeamColor color)
        {
            cavalry.Color = color;
            return this;
        }

        public IUnitBuilder SetId(Guid id)
        {
            cavalry.Id = id;
            return this;
        }

        public IUnitBuilder SetOpponentId(Guid? opponentId)
        {
            cavalry.OpponentId = opponentId;
            return this;
        }

        public IUnitBuilder SetOwnerId(Guid ownerId)
        {
            cavalry.OwnerId = ownerId;
            return this;
        }

        public IUnitBuilder SetPosition(Position position)
        {
            cavalry.Position = position;
            return this;
        }

        public IUnitBuilder SetMaxHealth(int maxHealth)
        {
            cavalry.MaxHealth = maxHealth;
            return this;
        }

        public IUnitBuilder SetProductionPrice(int productionPrice)
        {
            cavalry.ProductionPrice = productionPrice;
            return this;
        }

        public IUnitBuilder SetSpeed(int speed)
        {
            cavalry.Speed = speed;
            return this;
        }

        public IUnitBuilder SetBorderProperties(BorderProperties borderProperties)
        {
            cavalry.Border = borderProperties; 
            return this;
        }

        public Unit Build()
        {
            return cavalry;
        }

        public IUnitBuilder SetImage(string imageSource)
        {
            cavalry.ImageSource = imageSource;
            return this;
        }
    }
}
