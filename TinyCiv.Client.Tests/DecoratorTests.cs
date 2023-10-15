using TinyCiv.Client.Code.Units;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code;
using System.Windows.Controls;
using TinyCiv.Client.Code.BorderDecorators;
using System.Windows.Media;
using System.Windows;

namespace TinyCiv.Client.Tests
{
    public class DecoratorTests
    {
        public static GameObject GetWarrior()
        {
            Warrior warrior = new Warrior(GameObject.fromServerGameObject(new ServerGameObject
            {
                Color = TeamColor.Red,
                Id = new Guid(),
                OpponentId = null,
                OwnerPlayerId = new Guid(),
                Position = new ServerPosition
                {
                    X = 1,
                    Y = 1
                },
                Type = GameObjectType.Warrior,
            }));

            return warrior;
        }

        [Fact]
        public void HighlightDecoratorShouldApply()
        {
            GameObject warrior = GetWarrior();

            BorderDecorator decoratedWarrior = new BorderHighlightDecorator(warrior, Brushes.Red);
            var properties = decoratedWarrior.ApplyEffects();

            Assert.Equal(Brushes.Red, properties.BorderBrush);
            Assert.Equal(new Thickness(2), properties.BorderThickness);
        }

        [Fact]
        public void HighlightDecoratorShouldRemove()
        {
            GameObject warrior = GetWarrior();

            BorderDecorator decoratedWarrior = new BorderHighlightDecorator(warrior, Brushes.Red);
            decoratedWarrior.ApplyEffects();
            var properties = decoratedWarrior.RemoveEffects();

            Assert.Equal(new Thickness(0), properties.BorderThickness);
        }

        [Fact]
        public void BorderBackgroundDecoratorShouldApply()
        {
            GameObject warrior = GetWarrior();
            Brush expectedBrush = new SolidColorBrush(Color.FromArgb(64, 255, 0, 0));

            BorderDecorator decoratedWarrior = new BorderBackgroundDecorator(warrior, Brushes.Red);
            var properties = decoratedWarrior.ApplyEffects();

            Assert.Equal(expectedBrush.ToString(), properties.BackgroundBrush.ToString());
        }

        [Fact]
        public void BorderBackgroundDecoratorShouldRemove()
        {
            GameObject warrior = GetWarrior();

            BorderDecorator decoratedWarrior = new BorderBackgroundDecorator(warrior, Brushes.Red);
            decoratedWarrior.ApplyEffects();
            var properties = decoratedWarrior.RemoveEffects();

            Assert.Equal(Brushes.Transparent, properties.BackgroundBrush);
        }

    }
}