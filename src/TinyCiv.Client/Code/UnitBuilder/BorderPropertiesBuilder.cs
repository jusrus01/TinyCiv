using System.Windows.Media;
using System.Windows;
using TinyCiv.Client.Code.BorderDecorators;

namespace TinyCiv.Client.Code.UnitBuilder
{
    public class BorderPropertiesBuilder
    {
        private BorderProperties borderProperties = new BorderProperties();

        public BorderProperties Build()
        {
            return borderProperties;
        }

        public BorderPropertiesBuilder SetOpacity(double opacity)
        {
            borderProperties.Opacity = opacity;
            return this;
        }

        public BorderPropertiesBuilder SetBorderThickness(Thickness thickness)
        {
            borderProperties.BorderThickness = thickness;
            return this;
        }

        public BorderPropertiesBuilder SetBorderBrush(Brush brush)
        {
            borderProperties.BorderBrush = brush;
            return this;
        }

        public BorderPropertiesBuilder SetBackgroundBrush(Brush brush)
        {
            borderProperties.BackgroundBrush = brush;
            return this;
        }
    }

}
