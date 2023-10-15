using System.Windows.Controls;
using System.Windows.Media;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderBackgroundDecorator : BorderDecorator
    {
        private Color _color;
        public BorderBackgroundDecorator(IBorderObject decoratedObject, Color color) : base(decoratedObject)
        {
            _color = color;
        }

        public override BorderProperties ApplyEffects()
        {
            BorderProperties decoratedBorder = base.ApplyEffects();

            if (decoratedBorder != null)
            {
                _color = Color.FromArgb(128, _color.R, _color.G, _color.B);
                decoratedBorder.BackgroundBrush = new SolidColorBrush(_color);
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
