using System.Windows;
using System.Windows.Controls;
using TinyCiv.Client.Code.Decorators;

namespace TinyCiv.Client.Code.MVVM.View
{
    /// <summary>
    /// Interaction logic for UpperMenuView.xaml
    /// </summary>
    public partial class UpperMenuView : ToggleView
    {
        public UpperMenuView()
        {
            InitializeComponent();
        }

        public void Hide()
        {
            base.Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            base.Visibility = Visibility.Visible;
        }
    }
}
