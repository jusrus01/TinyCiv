using TinyCiv.Client.Code.UnitDecorators;
using System.Windows;
using System.Windows.Media;

namespace TinyCiv.Client.Code.Decorators
{
    public class BorderHighlightDecorator : BorderDecorator
    {
        private Color _color;

        public BorderHighlightDecorator(GameObject gameObject, Color color) : base(gameObject)
        {
            _color = color;
        }

        new public void ApplyBorderEffects()
        {
            base.ApplyBorderEffects();
            wrappee.BorderThickness = new Thickness(2);
            wrappee.BorderBrush = new SolidColorBrush(_color);
        }
    }
}
