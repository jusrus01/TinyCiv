using System.Windows.Controls;

namespace TinyCiv.Client.Code.MVVM.View.UserControls;

/// <summary>
/// Interaction logic for ToggleView.xaml
/// </summary>
public partial class ToggleView : UserControl
{
    public ToggleView()
    {
        InitializeComponent();
    }

    public virtual void Show() { }
    public virtual void Hide() { }
}
