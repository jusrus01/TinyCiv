using TinyCiv.Client.Code.UnitDecorators;
using System.Windows.Media;

namespace TinyCiv.Client.Code.Decorators
{
    public class BorderBackgroundDecorator : BorderDecorator
    {
        private Color _color;

        public BorderBackgroundDecorator(GameObject gameObject, Color color) : base(gameObject)
        {
            _color = color;
        }

        new public void ApplyBorderEffects()
        {
            base.ApplyBorderEffects();
            _color.A = 220;
            wrappee.BackgroundBrush = new SolidColorBrush(_color);
        }
    }
}
