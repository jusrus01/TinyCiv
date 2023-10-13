using TinyCiv.Client.Code.UnitDecorators;
using System.Windows;
using System.Windows.Media;

namespace TinyCiv.Client.Code.Decorators
{
    public class BorderHighlightDecorator : BorderDecorator
    {
        private Color _color;

        public BorderHighlightDecorator(BorderObject borderObject, Color color) : base(borderObject)
        {
            _color = color;
        }

        public override void ApplyBorderEffects()
        {
            base.ApplyBorderEffects();
            wrappee.BorderThickness = new Thickness(2);
            wrappee.BorderBrush = new SolidColorBrush(_color);
        }

        protected BorderObject GetWreppee()
        {          
            return wrappee;
        }
    }
}
