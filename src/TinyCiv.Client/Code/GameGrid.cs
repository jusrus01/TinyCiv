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

        private void DrawGameObjects()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                var obj = GameObjects[i];
            }
        }

        private async void Tile_Click(Position clickedPosition)
        {
            if (isUnitSelected)
            {
                var unit = (Unit)GameObjects[selectedUnitIndex];
                UnselectUnit(unit);
                await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(unit.Id, clickedPosition.column, clickedPosition.row));
            }
        }

        private void Unit_Click(GameObject gameObject)
        {
            var gameObjectIndex = gameObject.Position.column * Columns + gameObject.Position.row;

            if (!isUnitSelected && GameObjects[gameObjectIndex].OwnerId == CurrentPlayer.Id)
            {
                SelectUnit(gameObject, gameObjectIndex);
            }
            else if (isUnitSelected && gameObjectIndex == selectedUnitIndex)
            {
                UnselectUnit(gameObject);
            }
        }

        private void SelectUnit(GameObject gameObject, int gameObjectIndex)
        {
            isUnitSelected = true;
            selectedUnitIndex = gameObjectIndex;
            gameObject.BorderThickness = new Thickness(2);
            onPropertyChanged?.Invoke();
        }

        private void UnselectUnit(GameObject gameObject)
        {
            isUnitSelected = false;
            gameObject.BorderThickness = new Thickness(0);
            onPropertyChanged?.Invoke();
        }

        private async void Create_Unit(Position clickedPosition)
        {
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateUnitClientEvent(CurrentPlayer.Id, clickedPosition.row, clickedPosition.column));
        }

        private void OnMapChange(MapChangeServerEvent response)
        {
            var goFactory = new GameObjectFactory();
            GameObjects = response.Map.Objects
                //.Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => goFactory.Create(serverGameObect))
                .ToList<GameObject>();

            AddClickEvents();
            onPropertyChanged?.Invoke();
        }

        public void AddClickEvents()
        {
            foreach(var gameObject in GameObjects)
            {
                if (gameObject.Type == GameObjectType.Empty)
                {
                    gameObject.LeftAction = () => { Tile_Click(gameObject.Position); };
                    gameObject.RightAction = () => { Create_Unit(gameObject.Position); };
                }
                else if (gameObject.Type == GameObjectType.Warrior)
                {
                    gameObject.LeftAction = () => { Unit_Click(gameObject); };
                    gameObject.RightAction = () => { };
                }
            }
        }
    }
}
