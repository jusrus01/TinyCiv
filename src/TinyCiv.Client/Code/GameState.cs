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
using TinyCiv.Client.Code.MVVM.View;
using TinyCiv.Client.Code.MVVM.ViewModel;
using System.Data.Common;
using System.Reflection;


namespace TinyCiv.Client.Code
{
    public class GameState
    {
        public ObservableValue<List<Border>> SpriteList { get; } = new ObservableValue<List<Border>>();
        public Action onPropertyChanged;

        public List<string> mapImages = new List<string>();
        public List<GameObject> GameObjects = new List<GameObject>();
        public UnitMenuViewModel UnitMenuVM;
        private int Rows;
        private int Columns;

        private bool isUnitSelected = false;
        private GameObject selectedUnit;

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            ClientSingleton.Instance.serverClient.ListenForMapChange(OnMapChange);
        }

        private async void Tile_Click(Position clickedPosition)
        {
            if (isUnitSelected)
            {
                var unit = (Unit)selectedUnit;
                UnselectUnit(unit);
                await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(unit.Id, clickedPosition.row, clickedPosition.column));
            }
        }

        private async void Unit_Click(GameObject slectedGameObject)
        {
            var gameObjectIndex = slectedGameObject.Position.column * Columns + slectedGameObject.Position.row;

            if (!isUnitSelected && GameObjects[gameObjectIndex].OwnerId == CurrentPlayer.Id)
            {
                SelectUnit(slectedGameObject);
            } 
            else if (isUnitSelected && GameObjects[gameObjectIndex].OwnerId != CurrentPlayer.Id)
            {
                UnselectUnit(selectedUnit);
                await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(selectedUnit.Id, slectedGameObject.Position.row, slectedGameObject.Position.column));
            }
            else if (isUnitSelected && slectedGameObject == selectedUnit)
            {
                UnselectUnit(slectedGameObject);
            }
        }

        private void SelectUnit(GameObject gameObject)
        {
            isUnitSelected = true;
            selectedUnit = gameObject;
            gameObject.BorderBrush = Brushes.Aquamarine;
            gameObject.BorderThickness = new Thickness(2);
            UnitMenuVM.SetCurrentUnit(gameObject);
            onPropertyChanged?.Invoke();
        }

        private void UnselectUnit(GameObject gameObject)
        {
            isUnitSelected = false;
            gameObject.BorderThickness = new Thickness(0);
            UnitMenuVM.UnselectUnit();
            onPropertyChanged?.Invoke();
        }

        private void ShowCombatState(GameObject gameObject)
        {            
            if (gameObject.OpponentId != null)
            {
                gameObject.BorderThickness = new Thickness(2);
                gameObject.BorderBrush = Brushes.IndianRed;
            } 
        }

        private async void Create_Unit(Position clickedPosition)
        {
            await ClientSingleton.Instance.serverClient.SendAsync(new CreateUnitClientEvent(CurrentPlayer.Id, clickedPosition.row, clickedPosition.column));
        }

        private void OnMapChange(MapChangeServerEvent response)
        {
            var goFactory = new GameObjectFactory();
            var ResponseGameObjects = response.Map.Objects
                .Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => goFactory.Create(serverGameObect))
                .ToList<GameObject>();

            for(int row = 0; row < Rows; row++)
            {
                for(int column = 0; column < Columns; column++)
                {
                    var index = column * Columns + row;
                    GameObjects[index] = goFactory.Create(new ServerGameObject { Type = GameObjectType.Empty, Position = new ServerPosition() { X = row, Y = column} });
                }
            }

            foreach(var gameObject in ResponseGameObjects)
            {
                var gameObjectIndex = gameObject.Position.column * Columns + gameObject.Position.row;
                GameObjects[gameObjectIndex] = gameObject;
                ShowCombatState(gameObject);                
                if (isUnitSelected && selectedUnit.Id == gameObject.Id)
                {
                    SelectUnit(gameObject);
                }
            }
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
