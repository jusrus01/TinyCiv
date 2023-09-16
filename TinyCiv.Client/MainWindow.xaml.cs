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
        private Image[,] mapImages = new Image[20, 20];
        private GameObject[,] gameObjects = new GameObject[20, 20];
        private bool isUnitSelected = false;
        private Position unitPosition;


        public MainWindow()
        {
            gameObjects[5, 5] = new Unit(new BitmapImage(new Uri("Assets/warrior.png", UriKind.Relative)), 5, 5);

            gameObjects[5, 10] = new Unit(new BitmapImage(new Uri("Assets/warrior.png", UriKind.Relative)), 5, 10);


            InitializeComponent();
            InitializeMap();
            InitalizeUnitGrid();
        }

        private void InitalizeUnitGrid()
        {
            UnitGrid.Children.Clear();
            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 20; c++)
                {
                    Image image = new Image();
                    if (gameObjects[r, c] != null)
                    {
                        image.Source = new BitmapImage(new Uri("Assets/warrior.png", UriKind.Relative));
                    }
                    Border border = new Border();
                    border.Background = Brushes.Transparent;
                    border.Tag = new Position(r, c);
                    border.MouseDown += Tile_Click;
                    border.Child = image;
                    UnitGrid.Children.Add(border);
                }
            }
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

        private void Tile_Click(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var clickedPosition = border.Tag as Position;

            if (gameObjects[clickedPosition.row, clickedPosition.column] != null && !isUnitSelected)
            {
                isUnitSelected = true;
                unitPosition = clickedPosition;
            }
            else if (gameObjects[clickedPosition.row, clickedPosition.column] == null && isUnitSelected)
            {
                isUnitSelected = false;

                gameObjects[clickedPosition.row, clickedPosition.column] = gameObjects[unitPosition.row, unitPosition.column];
                gameObjects[unitPosition.row, unitPosition.column] = null;

            }

            InitalizeUnitGrid();
        }
    }
}
