using System.Windows.Media;
using System.Windows;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public class BorderProperties
    {
        public Thickness BorderThickness { get; set; }
        public Brush BorderBrush { get; set; }
        public Brush BackgroundBrush { get; set; }
        public double Opacity {  get; set; }
    }
}
