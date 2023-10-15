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
using TinyCiv.Client.Code.BorderDecorators;

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
                var unit = selectedUnit as Unit;
                await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(unit.Id, clickedPosition.row, clickedPosition.column));
                UnselectUnit(unit);
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

        private async void Unit_Click(GameObject selectedGameObject)
        {
            var gameObjectIndex = selectedGameObject.Position.column * Columns + selectedGameObject.Position.row;

            if (!isUnitSelected && GameObjects[gameObjectIndex].OwnerId == CurrentPlayer.Id)
            {
                SelectUnit(selectedGameObject);
            } 
            else if (isUnitSelected && GameObjects[gameObjectIndex].OwnerId != CurrentPlayer.Id)
            {
                await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(selectedUnit.Id, selectedGameObject.Position.row, selectedGameObject.Position.column));
                UnselectUnit(selectedUnit);
            }
            else if (isUnitSelected && selectedGameObject == selectedUnit)
            {
                UnselectUnit(selectedGameObject);
            }
        }

        private void SelectUnit(GameObject gameObject)
        {
            isUnitSelected = true;
            selectedUnit = gameObject;

            //gameObject.Border.BorderBrush = Brushes.Aquamarine;
            //gameObject.Border.BorderThickness = new Thickness(2);

            var decoratedObject = new BorderHighlightDecorator(gameObject, Brushes.Aquamarine);
            decoratedObject.ApplyEffects();

            UnitMenuVM.SetCurrentUnit(gameObject);
            onPropertyChanged?.Invoke();
        }

        private void UnselectUnit(GameObject gameObject)
        {
            isUnitSelected = false;
            //gameObject.BorderThickness = new Thickness(0);
            gameObject.RemoveEffects();
            //selectedUnit = null;
            UnitMenuVM.UnselectUnit();
            onPropertyChanged?.Invoke();
        }

        private void ShowCombatState(GameObject gameObject)
        {
            gameObject.RemoveEffects();

            var decoratedObject =
                new BorderFlashDecorator(
                    new BorderBackgroundDecorator(
                        new BorderHighlightDecorator(gameObject, Brushes.IndianRed), Colors.IndianRed));

            decoratedObject.ApplyEffects();
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
                .ToList();

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
                    var gameObject = TeamFactories[TeamColor.Red].CreateGameObject(serverGameObject);
                    AddClickEvent(gameObject);
                    GameObjects[index] = gameObject;
                }
            }

            foreach(var gameObject in ResponseGameObjects)
            {
                AddClickEvent(gameObject);
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
            //AddClickEvents();
            onPropertyChanged?.Invoke();
        }

        private void AddClickEvent(GameObject gameObject)
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
