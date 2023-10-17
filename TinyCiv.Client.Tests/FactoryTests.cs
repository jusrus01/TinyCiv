using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Tests
{
    public class FactoryTests
    {
        public ServerGameObject warrior = new ServerGameObject
        {
            Color = TeamColor.Red,
            Id = new Guid(),
            OpponentId = new Guid(),
            OwnerPlayerId = new Guid(),
            Position = new ServerPosition
            {
                X = 0,
                Y = 0,
            },
            Type = GameObjectType.Warrior
        };

        [Fact]
        public void RedFactoryShouldCreateRedWarrior()
        {
            RedGameObjectFactory factory = new RedGameObjectFactory();

            var unit = factory.CreateGameObject(warrior);

            Assert.IsType<Warrior>(unit);     
        }

        [Fact]
        public void RedFactoryWarriorShouldContainCorrectValues()
        {
            RedGameObjectFactory factory = new RedGameObjectFactory();

            var unit = factory.CreateGameObject(warrior);

            Assert.Equal(warrior.Id, unit.Id);
            Assert.Equal(warrior.Color, unit.Color);
            Assert.Equal(warrior.OwnerPlayerId, unit.OwnerId);
            Assert.Equal(warrior.Position.X, unit.Position.row);
            Assert.Equal(warrior.Position.Y, unit.Position.column);
            Assert.Equal(warrior.Type, unit.Type);
        }

    }
}
