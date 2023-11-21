//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Media;
//using TinyCiv.Client.Code.BorderDecorators;
//using TinyCiv.Client.Code.Factories;
//using TinyCiv.Client.Code.UnitBuilder;
//using TinyCiv.Client.Code;
//using TinyCiv.Shared.Game;

//namespace TinyCiv.Client.Tests
//{
//    public class WarriorBuilderTests
//    {
//        [Fact]
//        public void Build_WarriorBuilder_BuildsWarriorWithCorrectAttributes()
//        {
//            // Arrange
//            var builder = new WarriorBuilder();

//            var damage = 30;
//            var description = "Powerful warrior";
//            var expReward = 75;
//            var color = TeamColor.Red;
//            var id = Guid.NewGuid();
//            var opponentId = Guid.NewGuid();
//            var ownerId = Guid.NewGuid();
//            var position = new Position(1, 2);
//            var maxHealth = 100;
//            var productionPrice = 60;
//            var speed = 3;
//            var borderProperties = new BorderProperties { BackgroundBrush = Brushes.Blue };
//            var imageSource = "warrior_image.png";

//            // Act
//            var warrior = builder
//                .SetDamage(damage)
//                .SetDescription(description)
//                .SetExpReward(expReward)
//                .SetColor(color)
//                .SetId(id)
//                .SetOpponentId(opponentId)
//                .SetOwnerId(ownerId)
//                .SetPosition(position)
//                .SetMaxHealth(maxHealth)
//                .SetProductionPrice(productionPrice)
//                .SetSpeed(speed)
//                .SetBorderProperties(borderProperties)
//                .SetImage(imageSource)
//                .Build();

//            // Assert
//            Assert.NotNull(warrior);
//            Assert.Equal(damage, warrior.Damage);
//            Assert.Equal(description, warrior.Description);
//            Assert.Equal(expReward, warrior.ExpReward);
//            Assert.Equal(color, warrior.Color);
//            Assert.Equal(id, warrior.Id);
//            Assert.Equal(opponentId, warrior.OpponentId);
//            Assert.Equal(ownerId, warrior.OwnerId);
//            Assert.Equal(position, warrior.Position);
//            Assert.Equal(maxHealth, warrior.MaxHealth);
//            Assert.Equal(productionPrice, warrior.ProductionPrice);
//            Assert.Equal(speed, warrior.Speed);
//            Assert.Equal(borderProperties, warrior.Border);
//            Assert.Equal(imageSource, warrior.ImageSource);
//        }

//        [Fact]
//        public void Build_DefaultImageSource_UsesDefaultImageWhenImageSourceIsNull()
//        {
//            // Arrange
//            var builder = new WarriorBuilder();
//            string imageSource = null;

//            // Act
//            var warrior = builder.SetImage(imageSource).Build();

//            // Assert
//            Assert.NotNull(warrior);
//            Assert.Equal(
//                AbstractGameObjectFactory.getGameObjectImage(warrior.Color, warrior.Type),
//                warrior.ImageSource);
//        }
//    }
//}
