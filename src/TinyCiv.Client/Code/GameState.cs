using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TinyCiv.Client.Code.Units;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Shared.Events.Server;
using System.Linq;
using System;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM.ViewModel;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Client.Code.Decorators;
using System.Net.WebSockets;
using TinyCiv.Client.Code.UnitDecorators;

namespace TinyCiv.Client.Code
{
    public class GameState
    {
        public Dictionary<Guid, int> HealthValues; // : ))))))))))))))))))))))))))))))))))))))))))))))))))))))  HACKERMAN

        public ObservableValue<List<Border>> SpriteList { get; } = new ObservableValue<List<Border>>();
        public Action onPropertyChanged;

        public Resources Resources;
        public List<string> mapImages = new List<string>();
        public List<GameObject> GameObjects = new List<GameObject>();
        public UnitMenuViewModel UnitMenuVM;
        public UpperMenuViewModel UpperMenuVM;
        private int Rows;
        private int Columns;

        private bool isUnitSelected = false;
        private GameObject selectedUnit;

        private Dictionary<TeamColor, AbstractGameObjectFactory> TeamFactories = new Dictionary<TeamColor, AbstractGameObjectFactory>
        {
            {TeamColor.Red,    new RedGameObjectFactory() },
            {TeamColor.Green,  new GreenGameObjectFactory() },
            {TeamColor.Purple, new PurpleGameObjectFactory() },
            {TeamColor.Yellow, new YellowGameObjectFactory() }
        };

        public GameState(int rows, int columns)
        {
            HealthValues = new Dictionary<Guid, int>();
            Rows = rows;
            Columns = columns;
            ClientSingleton.Instance.serverClient.ListenForMapChange(OnMapChange);
            ClientSingleton.Instance.serverClient.ListenForInteractableObjectChanges(OnInteractableChange);
            ClientSingleton.Instance.serverClient.ListenForResourcesUpdate(OnResourceUpdate);
        }

        private void OnResourceUpdate(ResourcesUpdateServerEvent response)
        {
            Resources = response.Resources;
            UpperMenuVM.SetResources(Resources);
        }

        private async void Grass_Tile_Click(Position clickedPosition)
        {
            var buildingUnderPurchase = UnitMenuVM.SelectedBuyBuilding.Value;
            var unitUnderPurchase = UnitMenuVM.SelectedBuyUnit.Value;

            if (isUnitSelected)
            {
                var unit = (Unit)selectedUnit;
                UnselectUnit(unit);
                await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(unit.Id, clickedPosition.row, clickedPosition.column));
            }
            else if (unitUnderPurchase != null) 
            {
                UnitMenuVM.ExecuteUnitPurchase(clickedPosition);
            }
            else if (buildingUnderPurchase != null 
                && buildingUnderPurchase.Type != GameObjectType.Port 
                && buildingUnderPurchase.Type != GameObjectType.Mine)
            {
                UnitMenuVM.ExecuteBuildingPurchase(clickedPosition);
            }
        }

        private void Water_Tile_Click(Position clickedPosition)
        {
            var buildingUnderPurchase = UnitMenuVM.SelectedBuyBuilding.Value;
            if (buildingUnderPurchase != null && buildingUnderPurchase.Type == GameObjectType.Port)
            {
                UnitMenuVM.ExecuteBuildingPurchase(clickedPosition);
            }
        }

        private void Rock_Tile_Click(Position clickedPosition)
        {
            var buildingUnderPurchase = UnitMenuVM.SelectedBuyBuilding.Value;
            if (buildingUnderPurchase != null && buildingUnderPurchase.Type == GameObjectType.Mine)
            {
                UnitMenuVM.ExecuteBuildingPurchase(clickedPosition);
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

            var goHighlightedBorder = new BorderHighlightDecorator(gameObject, Colors.Aquamarine);
            var goDecoratedBG = new BorderBackgroundDecorator(goHighlightedBorder, Colors.Aquamarine);
            var goFlashBorder = new BorderFlashDecorator(goDecoratedBG);
            goFlashBorder.ApplyBorderEffects();

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
           gameObject.BorderThickness = new Thickness(2);
           gameObject.BorderBrush = Brushes.IndianRed;
        }

        private void OnInteractableChange(InteractableObjectServerEvent response)
        {
            for(int i = 0; i < GameObjects.Count; i++)
            {
                var gameObject = GameObjects[i];
                if (gameObject.Id == response.ObjectId) 
                {
                    var unit = (Unit)GameObjects[i];
                    if (unit != null)
                    {
                        unit.Health = response.Health;
                        
                        HealthValues[response.ObjectId] = response.Health;
                        onPropertyChanged?.Invoke();
                        return;
                    }
                }
            }
        }

        private void OnMapChange(MapChangeServerEvent response)
        {
            var ResponseGameObjects = response.Map.Objects
                .Where(serverGameObject => serverGameObject.Type != GameObjectType.Empty)
                .Select(serverGameObect => TeamFactories[serverGameObect.Color].CreateGameObject(serverGameObect))
                .ToList<GameObject>();

            for(int row = 0; row < Rows; row++)
            {
                for(int column = 0; column < Columns; column++)
                {
                    var index = column * Columns + row;
                    var serverGameObject = new ServerGameObject
                    {
                        Type = GameObjectType.Empty,
                        Position = new ServerPosition() { X = row, Y = column }
                    };
                    GameObjects[index] = TeamFactories[TeamColor.Red].CreateGameObject(serverGameObject);
                }
            }

            foreach(var gameObject in ResponseGameObjects)
            {
                var gameObjectIndex = gameObject.Position.column * Columns + gameObject.Position.row;
                GameObjects[gameObjectIndex] = gameObject;

                if (gameObject.OpponentId != null)
                {
                    ShowCombatState(gameObject);
                    Task.Run(() => ClientSingleton.Instance.serverClient.SendAsync(new AttackUnitClientEvent(gameObject.Id, gameObject.OpponentId.Value)));
                }
                
                if (isUnitSelected && selectedUnit.Id == gameObject.Id)
                {
                    SelectUnit(gameObject);
                }

                if (HealthValues.ContainsKey(gameObject.Id))
                {
                    ((Unit)gameObject).Health = HealthValues[gameObject.Id];
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
                    gameObject.LeftAction = () => { Grass_Tile_Click(gameObject.Position); };
                }
                else if (gameObject.Type == GameObjectType.StaticWater)
                {
                    gameObject.LeftAction = () => { Water_Tile_Click(gameObject.Position); };
                }
                else if (gameObject.Type == GameObjectType.StaticMountain)
                {
                    gameObject.LeftAction = () => { Rock_Tile_Click(gameObject.Position); };
                }
                else if (gameObject is Unit)
                {
                    gameObject.LeftAction = () => { Unit_Click(gameObject); };
                    gameObject.RightAction = () => { };
                }
            }
        }
    }
}
