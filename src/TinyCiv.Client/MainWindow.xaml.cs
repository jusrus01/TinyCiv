using System.Windows;
using System.Windows.Controls;
using TinyCiv.Client.Code.MVVM.View;
using TinyCiv.Client.Code.ViewDecorators;

namespace TinyCiv.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //var upperMenu = new UpperMenuView();
            //var decoratedMenu = new ViewBorderDecorator(upperMenu);

            //Grid.SetRow(decoratedMenu, 0);
            //sidePanelGrid.Children.Add(decoratedMenu);

            //decoratedMenu.Show();
        }             
    }
}
