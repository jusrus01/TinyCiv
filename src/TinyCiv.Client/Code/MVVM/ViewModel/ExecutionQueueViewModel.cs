using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TinyCiv.Client.Code.Commands;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Client.Code.MVVM.Model;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class ExecutionQueueViewModel : ObservableObject
    {
        public GameState gameState;
        public Action onPropertyChanged;
        public CommandInvoker CommandInvoker = new CommandInvoker();
        private DispatcherTimer timer;

        public ObservableCollection<ClockModel> ObjectsInQueue { get; } = new ObservableCollection<ClockModel>();
        public RelayCommand UndoCommand => new RelayCommand(HandleUndo, CanUndo);

        public ExecutionQueueViewModel()
        {
            gameState = HUDManager.mainVM.GameVM.gameState;
            UpdateClocks();
        }

        public void AddToQueue(ClockModel model)
        {
            ObjectsInQueue.Add(model);
        }

        private void UpdateClocks()
        {
            timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                if (ObjectsInQueue.Count > 0)
                {
                    var objectInQueue = ObjectsInQueue[0];
                    if (objectInQueue.BuyableObject.IsBuyable())
                    {
                        objectInQueue.RemainingTime = objectInQueue.RemainingTime.Subtract(TimeSpan.FromSeconds(1));
                        if (objectInQueue.RemainingTime < TimeSpan.Zero)
                        {
                            ObjectsInQueue.Remove(objectInQueue);
                        }
                    }
                }
                
                if (ObjectsInQueue.Count > CommandInvoker.GetCommandCount()) 
                {
                    ObjectsInQueue.RemoveAt(0);
                }

                CommandManager.InvalidateRequerySuggested();
            };

            timer.Start();
        }

        private bool CanUndo(object arg)
        {
            return ObjectsInQueue.Count > 0;
        }

        private void HandleUndo(object obj)
        {
            ObjectsInQueue.RemoveAt(ObjectsInQueue.Count - 1);
            Position undoPosition = CommandInvoker.UndoLastCommand().Position;

            int index = gameState.PositionToIndex(undoPosition);
            var tileReplacement = CreateDecoyReplacement(undoPosition);

            gameState.DecoyObjects.Remove(index);
            gameState.AddClickEvent(tileReplacement);
            gameState.GameObjects[index] = tileReplacement;
            gameState.onPropertyChanged?.Invoke();
        }

        private GameObject CreateDecoyReplacement(Position undoPosition)
        {
            int index = gameState.PositionToIndex(undoPosition);
            var currentObject = gameState.GameObjects[index];

            GameObjectType type = GameObjectType.Empty;
            if (currentObject.Type == GameObjectType.Mine)
            {
                type = GameObjectType.StaticMountain;
            }
            else if (currentObject.Type == GameObjectType.Port)
            {
                type = GameObjectType.StaticWater;
            }
            
            var serverGameObject = new ServerGameObject
            {
                Type = type,
                Position = new ServerPosition() { X = undoPosition.row, Y = undoPosition.column }
            };
            var redFactory = new RedGameObjectFactory();

            return redFactory.CreateGameObject(serverGameObject);
        }
    }
}
