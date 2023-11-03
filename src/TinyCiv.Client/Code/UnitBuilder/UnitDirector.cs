using System.Windows.Media;
using System.Windows;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.UnitBuilder
{
    public class UnitDirector
    {
        private IUnitBuilder builder;

        public void SetBuilder(IUnitBuilder builder)
        {
            this.builder = builder;
        }

        public Unit ConstructUnit(ServerGameObject rawObject)
        {
            return builder
                .SetId(rawObject.Id)
                .SetColor(rawObject.Color)
                .SetPosition(new Position(rawObject.Position))
                .SetImage()
                .SetOwnerId(rawObject.OwnerPlayerId)
                .SetOpponentId(rawObject.OpponentId)
                .SetBorderProperties(new BorderPropertiesBuilder()
                    .SetBackgroundBrush(Brushes.Transparent)
                    .SetBorderThickness(new Thickness(0))
                    .SetOpacity(1)
                    .Build())
                .Build();
        }

        public Unit ConstructUnitDecoyFor(GameObject rawObject)
        {
            return builder
                .SetPosition(rawObject.Position)
                .SetColor(rawObject.Color)
                .SetImage(rawObject.ImageSource)
                .SetMaxHealth(-1) // hides the healthbar
                .SetBorderProperties(new BorderPropertiesBuilder()
                    .SetBackgroundBrush(Brushes.Transparent)
                    .SetBorderThickness(new Thickness(0))
                    .SetOpacity(0.5)
                    .Build())
                .Build();
        }
    }
}
