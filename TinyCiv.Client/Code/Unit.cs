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
        public Unit(int r, int c)
        {
            imageSource = new BitmapImage(new Uri("Assets/warrior.png", UriKind.Relative));
            position = new Position(r, c);
        }

    }
}
