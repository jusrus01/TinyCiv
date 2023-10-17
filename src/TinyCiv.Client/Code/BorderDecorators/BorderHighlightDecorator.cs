using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderHighlightDecorator : BorderDecorator
    {
        private readonly Brush _color;

        public BorderHighlightDecorator(BorderObject decoratedObject, Brush color) : base(decoratedObject)
        {
            _color = color;
        }


        public override BorderProperties ApplyEffects()
        {
            BorderProperties decoratedBorder = base.ApplyEffects();

            if (decoratedBorder != null)
            {
                decoratedBorder.BorderThickness = new Thickness(2);
                decoratedBorder.BorderBrush = _color;                
            }

            return decoratedBorder;
        }
    }
}
