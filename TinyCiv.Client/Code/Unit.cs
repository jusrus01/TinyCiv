using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TinyCiv.Client.Code
{
    public class Unit : GameObject
    {
        public Border border;
        
        public Unit(BitmapImage imageSource, int r, int c) : base(r,c) 
        {
            border = new Border();
            border.BorderBrush = Brushes.Transparent;
            border.MouseDown += On_Click;

            Image image = new Image();
            if (imageSource != null )
            {
                image.Source = imageSource;
            }
            border.Child = image;
        }

        private void On_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(String.Format("Pressed {0},{1}", r, c));
        }
    }
}
