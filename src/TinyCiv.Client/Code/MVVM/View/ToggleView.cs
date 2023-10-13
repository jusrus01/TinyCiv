using System.Windows.Controls;

namespace TinyCiv.Client.Code.MVVM.View
{
    public abstract class ToggleView : UserControl
    {
        public abstract void Show();
        public abstract void Hide();
    }
}
