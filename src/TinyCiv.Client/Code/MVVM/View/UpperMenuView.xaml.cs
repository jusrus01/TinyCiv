using System.Windows;

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

        public override void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        public override void Show()
        {
            Visibility = Visibility.Visible;
        }
    }
}
