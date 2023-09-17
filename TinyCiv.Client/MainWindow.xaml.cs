using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TinyCiv.Client.Code;

namespace TinyCiv.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameGrid gameGrid;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMap();
            InitalizeUnitGrid();
        }

        private void InitalizeUnitGrid()
        {
            gameGrid = new GameGrid(UnitGrid,20,20);
            gameGrid.gameObjects.Add(new Unit(2, 0));
            gameGrid.gameObjects.Add(new Unit(2, 10));
            gameGrid.Update();
        }

        private void InitializeMap()
        {
            for (int r = 0; r < 20; r++)
            {
                for(int c = 0; c < 20; c++) {
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri("Assets/game_tile.png", UriKind.Relative));
                    MapGrid.Children.Add(image);
                }
            }            
        }
    }
}
