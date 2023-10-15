using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderHighlightDecorator : BorderDecorator
    {
        private readonly Color _color;

        public BorderHighlightDecorator(IBorderObject decoratedObject, Color color) : base(decoratedObject)
        {
            _color = color;
        }

        public override BorderProperties ApplyEffects()
        {
            BorderProperties decoratedBorder = base.ApplyEffects();

            if (decoratedBorder != null)
            {
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                    decoratedBorder.BorderThickness = new Thickness(2);
                    decoratedBorder.BorderBrush = new SolidColorBrush(_color);
                //});
            }

            return decoratedBorder;
        }
    }
}
