using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public abstract class BorderDecorator : BorderObject
    {
        protected BorderObject _decoratedObject;
        public override Position Position { get => _decoratedObject.Position; set => _decoratedObject.Position = value; }


        protected BorderDecorator(BorderObject decoratedObject)
        {
            _decoratedObject = decoratedObject;
        }

        public override BorderProperties ApplyEffects()
        {
            return _decoratedObject.ApplyEffects();
        }

        public override BorderProperties RemoveEffects()
        {
            return _decoratedObject.RemoveEffects();
        }

        public Color ConvertBrushToColor(Brush brush)
        {
            if (brush is SolidColorBrush solidColorBrush)
            {
                return solidColorBrush.Color;
            }
            return Colors.Transparent;
        }

        public SolidColorBrush ConvertColorToBrush(Color color)
        {
            SolidColorBrush brush = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                brush = new SolidColorBrush(color);
            });

            return brush;
        }
    }
}
