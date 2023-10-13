using System.Windows.Media;
using System.Windows;

namespace TinyCiv.Client.Code.Decorators;

public abstract class BorderObject
{
    public virtual Thickness BorderThickness { get; set; }
    public virtual Brush BorderBrush { get; set; }
    public virtual Brush BackgroundBrush { get; set; }

    public abstract void ApplyBorderEffects();
    public abstract void RemoveBorderEffects();
}
