using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TinyCiv.Client.Code.MVVM.View.UserControls
{
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
}
