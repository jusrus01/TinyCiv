using System.Windows.Media;
using System.Windows;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderProperties
    {
        public Thickness BorderThickness { get; set; } = new Thickness(0);
        public Brush BorderBrush { get; set; } = Brushes.Transparent;
        public Brush BackgroundBrush { get; set; } = Brushes.Transparent;
    }
}
