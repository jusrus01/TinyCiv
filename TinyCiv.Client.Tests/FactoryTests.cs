using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Client.Code.Units;
using TinyCiv.Client.Code;

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

        [Theory, MemberData(nameof(ServerGameObjectTypeData))]
        public void GreenFactoryCreationShouldBeCorrect(ServerGameObject serverGameObject)
        {
            var factory = new GreenGameObjectFactory();

            var unit = factory.CreateGameObject(serverGameObject);

            Assert.Equal(serverGameObject.Id, unit.Id);
            Assert.Equal(serverGameObject.Color, unit.Color);
            Assert.Equal(serverGameObject.OwnerPlayerId, unit.OwnerId);
            Assert.Equal(serverGameObject.Position.X, unit.Position.row);
            Assert.Equal(serverGameObject.Position.Y, unit.Position.column);
            Assert.Equal(serverGameObject.Type, unit.Type);
        }

        [Theory]
        [InlineData(GameObjectType.Warrior)]
        [InlineData(GameObjectType.Colonist)]
        [InlineData(GameObjectType.Cavalry)]
        [InlineData(GameObjectType.Tarran)]
        [InlineData(GameObjectType.Farm)]
        public void GreenFactoryDecoysShouldBeCorrect(GameObjectType type)
        {
            var factory = new GreenGameObjectFactory();
            var position = new Position(0, 0);

            var decoy = factory.CreateObjectDecoy(type, position);

            Assert.Equal(TeamColor.Green, decoy.Color);
            Assert.Equal(0, decoy.Position.row);
            Assert.Equal(0, decoy.Position.column);
        }

        public static IEnumerable<object[]> ServerGameObjectTypeData()
        {
            yield return new object[]
            {
                    new ServerGameObject
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
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Cavalry
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Mine
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.City
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Tarran
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Bank
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Blacksmith
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Shop
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Port
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Farm
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Colonist
                    }
            };

            yield return new object[]
            {
                    new ServerGameObject
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
                        Type = GameObjectType.Empty
                    }
            };
        }

    }
}
