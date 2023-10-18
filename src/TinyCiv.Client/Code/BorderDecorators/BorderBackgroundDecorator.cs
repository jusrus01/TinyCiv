using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderBackgroundDecorator : BorderDecorator
    {
        private Brush _color;
        public BorderBackgroundDecorator(BorderObject decoratedObject, Brush color) : base(decoratedObject)
        {
            _color = color;
        }

        public override BorderProperties ApplyEffects()
        {
            BorderProperties decoratedBorder = base.ApplyEffects();

            if (decoratedBorder != null)
            {
                Color brushColor = ConvertBrushToColor(_color);
                var transparentColor = Color.FromArgb(64, brushColor.R, brushColor.G, brushColor.B);
                decoratedBorder.BackgroundBrush = new SolidColorBrush(transparentColor);
            }

            return decoratedBorder;
        }

        public override BorderProperties RemoveEffects()
        {
            BorderProperties decoratedBorder = base.RemoveEffects();

            if (decoratedBorder != null)
            {
                decoratedBorder.BackgroundBrush = Brushes.Transparent;
            }

            return decoratedBorder;
        }
    }
}
