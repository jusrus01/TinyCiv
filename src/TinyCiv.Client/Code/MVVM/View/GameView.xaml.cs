using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace TinyCiv.Client.Code.MVVM.View
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();

            itemsControl.ItemTemplateSelector = new GameObjectTemplateSelector
            {
                UnitTemplate = (DataTemplate)Resources["UnitTemplate"],
                GameObjectTemplate = (DataTemplate)Resources["GameObjectTemplate"],
            };
        }

        private void gameObjectLeftDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                var gameObject = border.DataContext as GameObject;
                gameObject?.LeftAction?.Invoke();
            }
        }

        private void gameObjectRightDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                var gameObject = border.DataContext as GameObject;
                gameObject?.RightAction?.Invoke();
            }
        }

    }
}
