using System.Collections.Generic;
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
using System.Threading;
using TinyCiv.Client.Code.Structures;
using TinyCiv.Client.Code.UnitBuilder;

namespace TinyCiv.Client.Code
{
    public class GameState
    {
        public Dictionary<Guid, int> HealthValues; // : ))))))))))))))))))))))))))))))))))))))))))))))))))))))  HACKERMAN

        public ObservableValue<List<Border>> SpriteList { get; } = new ObservableValue<List<Border>>();
        public Action onPropertyChanged;

        public List<string> mapImages = new List<string>();
        public List<GameObject> GameObjects = new List<GameObject>();
        public Dictionary<int, GameObject> DecoyObjects = new();
        public UnitMenuViewModel UnitMenuVM;
        public UpperMenuViewModel UpperMenuVM;
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public bool isGameObjectSelected = false;
        private GameObject selectedGameObject;

        private Dictionary<TeamColor, AbstractGameObjectFactory> TeamFactories = new()
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

            GameObjects = new List<GameObject>(Rows*Columns);
            Thread AddListenersThread = new Thread(() =>
            {
                ClientSingleton.Instance.WaitForInitialization();
                ClientSingleton.Instance.serverClient.ListenForMapChange(OnMapChange);
                ClientSingleton.Instance.serverClient.ListenForInteractableObjectChanges(OnInteractableChange);
                //ClientSingleton.Instance.serverClient.ListenForNewUnitCreation(OnUnitCreation);
            });
            AddListenersThread.Start();
        }

        private async void Grass_Tile_Click(Position clickedPosition)
        {
            if (isGameObjectSelected && selectedGameObject is Unit)
            {
                await MoveUnit(clickedPosition);
            }
            if (isGameObjectSelected && selectedGameObject is City)
            {
                var hud = HUDManager.Instance;
                var buildingUnderPurchase = hud.GetSelectedBuyBuilding();
                var unitUnderPurchase = hud.GetSelectedBuyUnit();
                if (unitUnderPurchase != null)
                {
                    InvokeUnitSpawnProcess(clickedPosition);
                }
                else if (buildingUnderPurchase != null
                    && buildingUnderPurchase.Type != GameObjectType.Port
                    && buildingUnderPurchase.Type != GameObjectType.Mine)
                {
                    InvokeBuildingProcess(clickedPosition);
                }
            }
        }

        private void Water_Tile_Click(Position clickedPosition)
        {
            var buildingUnderPurchase = HUDManager.Instance.GetSelectedBuyBuilding();
            if (buildingUnderPurchase != null && buildingUnderPurchase.Type == GameObjectType.Port)
            {
                InvokeBuildingProcess(clickedPosition);
            }
        }

        private void Rock_Tile_Click(Position clickedPosition)
        {
            var buildingUnderPurchase = HUDManager.Instance.GetSelectedBuyBuilding();
            if (buildingUnderPurchase != null && buildingUnderPurchase.Type == GameObjectType.Mine)
            {
                InvokeBuildingProcess(clickedPosition);
            }
        }

        private void InvokeBuildingProcess(Position position)
        {
            if (!InRangeOfCity(position)) return;
            var buildingUnderPurchase = HUDManager.Instance.GetSelectedBuyBuilding();
            AddDecoy(buildingUnderPurchase.Type, position);
            HUDManager.Instance.ExecuteBuildingPurchase(position);
            onPropertyChanged?.Invoke();
        }

        private bool InRangeOfCity(Position clickPosition)
        {
            Position cityPosition = GetCityPosition();
            var difference = cityPosition - clickPosition;
            if (Math.Abs(difference.row) >= Shared.Constants.Game.BuildingSpaceFromTown ||
                Math.Abs(difference.column) >= Shared.Constants.Game.BuildingSpaceFromTown)
            {
                return false;
            }
            return true;
        }

        private Position GetCityPosition()
        {
            if (selectedGameObject is City && CurrentPlayer.IsOwner(selectedGameObject))
                return selectedGameObject.Position;
            return GameObjects.Where(go => CurrentPlayer.IsOwner(go) && go.Type == GameObjectType.City).FirstOrDefault().Position;
        }

        private void InvokeUnitSpawnProcess(Position position)
        {
            if (!InRangeOfCity(position)) return;
            var unitUnderPurchase = HUDManager.Instance.GetSelectedBuyUnit();
            AddDecoy(unitUnderPurchase.Type, position);
            HUDManager.Instance.ExecuteUnitPurchase(position);
            onPropertyChanged?.Invoke();
        }

        private async void Unit_Click(GameObject selectedGameObject)
        {
            var gameObjectIndex = selectedGameObject.Position.column * Columns + selectedGameObject.Position.row;

            if (!isGameObjectSelected)
            {
                SelectUnit(selectedGameObject);
            } 
            else if (isGameObjectSelected && !CurrentPlayer.IsOwner(GameObjects[gameObjectIndex]) && selectedGameObject is Unit)
            {
                await MoveUnit(selectedGameObject.Position);
            }
            else if (isGameObjectSelected && selectedGameObject == this.selectedGameObject)
            {
                UnselectUnit(selectedGameObject);
            }
        }

        private async void City_Click(GameObject gameObject) 
        {
            if (!isGameObjectSelected)
            {
                SelectCity(gameObject);
            }
            else if (isGameObjectSelected && CurrentPlayer.IsOwner(gameObject))
            {
                UnselectCity(gameObject);
            }
            else if (isGameObjectSelected && selectedGameObject is Unit)
            {
                await MoveUnit(gameObject.Position);
            }
        }

        private void UnselectCity(GameObject gameObject)
        {
            isGameObjectSelected = false;
            (gameObject as City).RemoveHighlight();
            gameObject.RemoveEffects();
            HUDManager.Instance.HideLowerMenu();
            onPropertyChanged?.Invoke();
        }

        protected virtual void SelectCity(GameObject gameObject)
        {
            isGameObjectSelected = true;
            selectedGameObject = gameObject;

            BorderDecorator decoratedCity = new BorderHighlightDecorator(gameObject, Brushes.DarkSalmon);
            decoratedCity = new BorderBackgroundDecorator(decoratedCity, Brushes.DarkSalmon);
            decoratedCity = new BorderAreaDecorator(decoratedCity, Shared.Constants.Game.BuildingSpaceFromTown, GameObjects);

            (gameObject as City).HighlightedArea = (decoratedCity as BorderAreaDecorator).highlightedGameObjects;

            decoratedCity.ApplyEffects();
            HUDManager.Instance.DisplayCityMenu();
            onPropertyChanged?.Invoke();
        }

        private async Task MoveUnit(Position clickedPosition)
        {
            var unit = selectedGameObject as Unit;
            await unit.MoveTo(clickedPosition);
            UnselectUnit(unit);
        }

        protected virtual void SelectUnit(GameObject gameObject)
        {
            isGameObjectSelected = true;
            selectedGameObject = gameObject;

            BorderDecorator decoratedObject = new BorderHighlightDecorator(gameObject, Brushes.Aquamarine);
            decoratedObject = new BorderBackgroundDecorator(decoratedObject, Brushes.Aquamarine);

            decoratedObject.ApplyEffects();

            HUDManager.Instance.DisplayUnit((Unit)gameObject);
            onPropertyChanged?.Invoke();
        }

        private void UnselectUnit(GameObject gameObject)
        {
            isGameObjectSelected = false;
            gameObject.RemoveEffects();
            HUDManager.Instance.HideLowerMenu();
            onPropertyChanged?.Invoke();
        }

        private void ShowCombatState(GameObject gameObject)
        {
            gameObject.RemoveEffects();

            BorderDecorator decoratedObject = new BorderHighlightDecorator(gameObject, Brushes.IndianRed);
            decoratedObject = new BorderBackgroundDecorator(decoratedObject, Brushes.IndianRed);

            decoratedObject.ApplyEffects();
        }

        private void OnInteractableChange(InteractableObjectServerEvent response)
        {
            for(int i = 0; i < GameObjects.Count; i++)
            {
                var gameObject = GameObjects[i];
                if (gameObject.Id == response.ObjectId)
                {
                    if (GameObjects[i] is Unit)
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
        }

        private void OnUnitCreation(CreateUnitServerEvent response)
        {
            var sGameObject = response.CreatedUnit;
            var unit = TeamFactories[sGameObject.Color].CreateGameObject(sGameObject);
            var gameObjectIndex = unit.Position.column * Columns + unit.Position.row;
            GameObjects[gameObjectIndex] = unit;
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

            foreach (var gameObject in ResponseGameObjects)
            {
                AddClickEvent(gameObject);
                var gameObjectIndex = gameObject.Position.column * Columns + gameObject.Position.row;
                if (gameObject.Type != GameObjectType.StaticMountain && gameObject.Type != GameObjectType.StaticWater)
                {
                    RemoveDecoyAt(gameObjectIndex);
                }
                GameObjects[gameObjectIndex] = gameObject;

                if (gameObject.OpponentId != null)
                {
                    ShowCombatState(gameObject);
                    Task.Run(() => ClientSingleton.Instance.serverClient.SendAsync(new AttackUnitClientEvent(gameObject.OwnerId, gameObject.Id, gameObject.OpponentId.Value)));
                }
                
                if (isGameObjectSelected && selectedGameObject.Id == gameObject.Id)
                {
                    if (gameObject is Unit)
                        SelectUnit(gameObject);
                    else if (gameObject is City)
                        SelectCity(gameObject);
                }

                if (HealthValues.ContainsKey(gameObject.Id))
                {
                    ((Unit)gameObject).Health = HealthValues[gameObject.Id];
                }
            }

            ShowDecoys();

            onPropertyChanged?.Invoke();
        }

        private void ShowDecoys()
        {
            foreach (var go in DecoyObjects.Values)
            {
                var goIndex = go.Position.column * Columns + go.Position.row;
                GameObjects[goIndex] = go;
            }
        }

        private void RemoveDecoyAt(int index)
        {
            if (DecoyObjects.ContainsKey(index))
            {
                DecoyObjects.Remove(index);
            }
        }

        private void AddDecoy(GameObjectType type, Position position)
        {
            var goIndex = position.column * Columns + position.row;
            var decoy = TeamFactories[CurrentPlayer.Color].CreateObjectDecoy(type, position);
            //var decoy = new GameObject(type, position, CurrentPlayer.Color, 0.5);
            DecoyObjects.Add(goIndex, decoy);
            GameObjects[goIndex] = decoy;
        }

        public int PositionToIndex(Position position)
        {
            return position.column * Columns + position.row;
        }

        public void AddClickEvent(GameObject gameObject)
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
            }
            else if (gameObject is City)
            {
                gameObject.LeftAction = () => { City_Click(gameObject); };
            }
        }

        public void AddClickEvents()
        {
            foreach(var gameObject in GameObjects)
            {
                AddClickEvent(gameObject);
            }
        }
    }
}
