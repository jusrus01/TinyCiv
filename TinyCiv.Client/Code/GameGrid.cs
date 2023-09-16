using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TinyCiv.Client.Code
{
    public class GameGrid
    {
        public UniformGrid SpriteGrid { get; set; }
        public int rowCount;
        public int columnCount;
        public List<GameObject> gameObjects = new List<GameObject>();

        private bool isUnitSelected = false;
        private int unitIndex;
        
        public GameGrid(UniformGrid uniformGrid, int r, int c)
        {
            SpriteGrid = uniformGrid;
            rowCount = r;
            columnCount = c;
        }

        //Redraws the entire grid
        public void Update()
        {
            //Clears and sets up empty images
            SpriteGrid.Children.Clear();
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < columnCount; c++)
                {
                    Image image = new Image();
                    Border border = new Border();
                    border.Background = Brushes.Transparent;
                    border.Tag = new Position(r, c);
                    border.MouseDown += Tile_Click;
                    border.Child = image;
                    SpriteGrid.Children.Add(border);
                }
            }

            //For every game object, insert image
            for(int i = 0; i < gameObjects.Count; i++)
            {
                var imageSource = gameObjects[i].imageSource;
                int indexPosition = gameObjects[i].position.row * columnCount + gameObjects[i].position.column;
                var border = (Border)SpriteGrid.Children[indexPosition];
                border.Tag = i;
                border.MouseDown -= Tile_Click;
                border.MouseDown += Unit_Click;
                var image = (Image)border.Child;
                image.Source = imageSource;
                if (isUnitSelected && unitIndex == i)
                {
                    border.BorderBrush = Brushes.Aquamarine;
                    border.BorderThickness = new Thickness(2);
                }
            }
        }

        private void Tile_Click(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var clickedPosition = border.Tag as Position;
            
            if (isUnitSelected)
            {
                isUnitSelected = false;
                var unit = (Unit)gameObjects[unitIndex];
                unit.onUpdate = Update;
                unit.moveTowards(clickedPosition);
                Update();
            }
        }

        private void Unit_Click(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var gameObjectIndex = (int)border.Tag;
            var position = gameObjects[gameObjectIndex].position;

            if (!isUnitSelected)
            {
                isUnitSelected = true;
                unitIndex = gameObjectIndex;
                Update();
            }
            else if (isUnitSelected && gameObjectIndex == unitIndex) 
            {
                isUnitSelected = false;
                Update();
            }
        }
    }
}
