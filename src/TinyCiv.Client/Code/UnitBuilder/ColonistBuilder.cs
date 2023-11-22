using System;
using System.Windows.Controls;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Client.Code.units;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;
using static TinyCiv.Shared.Constants.Game.Interactable;

namespace TinyCiv.Client.Code.UnitBuilder
{
    public class ColonistBuilder : IUnitBuilder
    {
        private Colonist colonist;

        public ColonistBuilder()
        {
            colonist = new Colonist();
        }

        public IUnitBuilder SetDamage(int damage)
        {
            colonist.Damage = damage;
            return this;
        }

        public IUnitBuilder SetDescription(string description)
        {
            colonist.Description = description;
            return this;
        }

        public IUnitBuilder SetExpReward(int expReward)
        {
            colonist.ExpReward = expReward;
            return this;
        }

        public IUnitBuilder SetGameObjectColor(TeamColor color)
        {
            colonist.Color = color;
            return this;
        }

        public IUnitBuilder SetGameObjectId(Guid id)
        {
            colonist.Id = id;
            return this;
        }

        public IUnitBuilder SetGameObjectOpacity(double opacity)
        {
            colonist.Border.Opacity = opacity;
            return this;
        }

        public IUnitBuilder SetGameObjectOpponentId(Guid? opponentId)
        {
            colonist.OpponentId = opponentId;
            return this;
        }

        public IUnitBuilder SetGameObjectOwnerId(Guid ownerId)
        {
            colonist.OwnerId = ownerId;
            return this;
        }

        public IUnitBuilder SetGameObjectPosition(Position position)
        {
            colonist.Position = position;
            return this;
        }

        public IUnitBuilder SetMaxHealth(int maxHealth)
        {
            colonist.MaxHealth = maxHealth;
            return this;
        }

        public IUnitBuilder SetProductionPrice(int productionPrice)
        {
            colonist.ProductionPrice = productionPrice;
            return this;
        }

        public IUnitBuilder SetSpeed(int speed)
        {
            colonist.Speed = speed;
            return this;
        }

        public IUnitBuilder SetPosition(Position position)
        {
            colonist.Position = position;
            return this;
        }

        public IUnitBuilder SetOwnerId(Guid ownerId)
        {
            colonist.OwnerId = ownerId;
            return this;
        }

        public IUnitBuilder SetId(Guid id)
        {
            colonist.Id = id;
            return this;
        }

        public IUnitBuilder SetColor(TeamColor color)
        {
            colonist.Color = color;
            return this;
        }

        public IUnitBuilder SetOpponentId(Guid? opponentId)
        {
            colonist.OpponentId = opponentId;
            return this;
        }

        public IUnitBuilder SetBorderProperties(BorderProperties borderProperties)
        {
            colonist.Border = borderProperties;
            return this;
        }

        public Unit Build()
        {
            return colonist;
        }

        public IUnitBuilder SetImage(Image imageSource)
        {
            if (imageSource == null)
                colonist.ImageSource = AbstractGameObjectFactory.getGameObjectImage(colonist.Color, colonist.Type);
            else colonist.ImageSource = imageSource;

            return this;
        }
    }
}
