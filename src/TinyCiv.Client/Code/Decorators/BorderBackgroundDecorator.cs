using TinyCiv.Client.Code.UnitDecorators;
using System.Windows.Media;
using System.Windows;

namespace TinyCiv.Client.Code.Decorators
{
    public class BorderBackgroundDecorator : BorderDecorator
    {
        private Color _color;

        public BorderBackgroundDecorator(BorderObject borderObject, Color color) : base(borderObject)
        {
            _color = color;
        }

        public override void ApplyBorderEffects()
        {
            base.ApplyBorderEffects();
            _color.A = 64;
            wrappee.BackgroundBrush = new SolidColorBrush(_color);
        }
    }
}
