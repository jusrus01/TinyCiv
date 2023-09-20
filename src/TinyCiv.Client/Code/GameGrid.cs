using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TinyCiv.Client.Code.Units;
using TinyCiv.Server.Client;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameGrid
    {
        public UniformGrid SpriteGrid { get; set; }
        public List<GameObject> GameObjects = new List<GameObject>();
        public IServerClient Client;
        public Player CurrentPlayer;
        private int Rows;
        private int Columns;

        private bool isUnitSelected = false;
        private int selectedUnitIndex;

        public GameGrid(UniformGrid uniformGrid, int rows, int columns)
        {
            SpriteGrid = uniformGrid;
            Rows = rows;
            Columns = columns;
        }

        // Redraws the entire grid
        public void Update()
        {
            SpriteGrid.Children.Clear();

            CreateClickableTiles();

            DrawGameObjects();
        }

        private void CreateClickableTiles()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var position = new Position(row, col);
                    var border = CreateEmptyBorder(position);
                    SpriteGrid.Children.Add(border);
                }
            }
        }

        private void DrawGameObjects()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                var gameObject = GameObjects[i];
                var indexPosition = gameObject.Position;
                var border = (Border)SpriteGrid.Children[indexPosition.row * Columns + indexPosition.column];
                border.Tag = i;
                border.MouseLeftButtonDown -= Tile_Click;
                border.MouseRightButtonDown -= Create_Unit;
                border.MouseLeftButtonDown += Unit_Click;
                var image = (Image)border.Child;
                image.Source = Images.GetImage(gameObject);
                if (isUnitSelected && selectedUnitIndex == i)
                {
                    border.BorderBrush = Brushes.Aquamarine;
                    border.BorderThickness = new Thickness(2);
                }
            }
        }

        private Border CreateEmptyBorder(Position position)
        {
            var image = new Image();
            var border = new Border
            {
                Background = Brushes.Transparent,
                Tag = position
            };
            border.MouseLeftButtonDown += Tile_Click;
            border.MouseRightButtonDown += Create_Unit;
            border.Child = image;
            return border;
        }

        private async void Tile_Click(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var clickedPosition = (Position)border.Tag;

            if (isUnitSelected)
            {
                isUnitSelected = false;
                var unit = (Unit)GameObjects[selectedUnitIndex];
                await Client.SendAsync(new MoveUnitClientEvent(unit.Id, clickedPosition.row, clickedPosition.column));
            }
        }

        private void Unit_Click(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var gameObjectIndex = (int)border.Tag;

            if (!isUnitSelected)
            {
                isUnitSelected = true;
                selectedUnitIndex = gameObjectIndex;
                Update();
            }
            else if (isUnitSelected && gameObjectIndex == selectedUnitIndex)
            {
                isUnitSelected = false;
                Update();
            }
        }

        private async void Create_Unit(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var clickedPosition = (Position)border.Tag;

            await Client.SendAsync(new AddNewUnitClientEvent(CurrentPlayer.Id, clickedPosition.row, clickedPosition.column));
        }
    }
}
