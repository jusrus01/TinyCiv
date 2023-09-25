using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TinyCiv.Client.Code.Units;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Shared.Events.Server;
using System.Linq;
using System;
using System.Windows.Media.Imaging;
using TinyCiv.Shared;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using TinyCiv.Client.Code.Core;

namespace TinyCiv.Client.Code
{
    public class GameGrid
    {
        public ObservableValue<List<Border>> SpriteList { get; } = new ObservableValue<List<Border>>();
        public Action onPropertyChanged;

        public List<string> mapImages = new List<string>();
        //public UniformGrid MapList;
        //public ObservableValue<List<Image>> MapList { get; } = new ObservableValue<List<Image>>();
        //public ObservableValue<ObservableCollection<string>> MapList { get; } = new ObservableValue<ObservableCollection<string>>();
        public List<GameObject> GameObjects = new List<GameObject>();
        private int Rows;
        private int Columns;

        private bool isUnitSelected = false;
        private int selectedUnitIndex;

        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            ClientSingleton.Instance.serverClient.ListenForMapChange(OnMapChange);
            CreateMap();
        }

        private void CreateMap()
        {
            var list = new List<string>();

            for (int i = 0; i < Rows * Columns; i++) 
            {
                list.Add("/assets/game_tile.png");
            }
            mapImages = list;
        }

        // Redraws the entire grid
        public void Update()
        {
            CreateClickableTiles();
            DrawGameObjects();
            onPropertyChanged?.Invoke();
        }

        private void CreateClickableTiles()
        {
            var list = new List<Border>();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var position = new Position(row, col);
                    var border = CreateEmptyBorder(position);
                    list.Add(border);
                }
            }
            SpriteList.Value = list;
        }

        private void DrawGameObjects()
        {
            var list = SpriteList.Value;
            for (int i = 0; i < GameObjects.Count; i++)
            {
                var gameObject = GameObjects[i];
                var indexPosition = gameObject.Position;
                var border = list[indexPosition.row * Columns + indexPosition.column];
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
            SpriteList.Value = list;
        }

        private Border CreateEmptyBorder(Position position)
        {
            var border = Application.Current.Dispatcher.Invoke(() =>
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
            });
            return border;
        }

        private async void Tile_Click(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var clickedPosition = (Position)border.Tag;

            if (isUnitSelected)
            {
                UnselectUnit();
                var unit = (Unit)GameObjects[selectedUnitIndex];
                await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(unit.Id, clickedPosition.row, clickedPosition.column));
            }
        }

        private void Unit_Click(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var gameObjectIndex = (int)border.Tag;

            if (!isUnitSelected && GameObjects[gameObjectIndex].OwnerId == CurrentPlayer.Id)
            {
                isUnitSelected = true;
                selectedUnitIndex = gameObjectIndex;
                //ViewModel.UnitName.Value = typeof(Unit).ToString();
                //ViewModel.IsUnitStatVisible.Value = "Visible";
                Update();
            }
            else if (isUnitSelected && gameObjectIndex == selectedUnitIndex)
            {
                UnselectUnit();
            }
        }

        private void UnselectUnit()
        {
            isUnitSelected = false;
            //ViewModel.UnitName.Value = "NULL";
            //ViewModel.IsUnitStatVisible.Value = "Hidden";
            Update();
        }

        private async void Create_Unit(object sender, MouseButtonEventArgs e)
        {
            var border = (Border)sender;
            var clickedPosition = (Position)border.Tag;

            await ClientSingleton.Instance.serverClient.SendAsync(new CreateUnitClientEvent(CurrentPlayer.Id, clickedPosition.row, clickedPosition.column));
        }

        private void OnMapChange(MapChangeServerEvent response)
        {
            GameObjects = response.Map.Objects
                .Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => new Warrior(serverGameObect))
                .ToList<GameObject>();

            Update();
        }
    }
}
