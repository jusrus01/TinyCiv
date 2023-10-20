using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public CommandInvoker CommandInvoker = new CommandInvoker();

        public ObservableCollection<ClockModel> ObjectsInQueue { get; } = new ObservableCollection<ClockModel>();
        public RelayCommand UndoCommand => new RelayCommand(HandleUndo, CanUndo);

        public ExecutionQueueViewModel()
        {
            UpdateClocks();
        }          

        private void UpdateClocks()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
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
            gameState.DecoyObjects.Remove(index);
            var serverGameObject = new ServerGameObject
            {
                Type = GameObjectType.Empty,
                Position = new ServerPosition() { X = undoPosition.row, Y = undoPosition.column }
            };
            var redFactory = new RedGameObjectFactory();
            var tileReplacement = redFactory.CreateGameObject(serverGameObject);
            gameState.AddClickEvent(tileReplacement);
            gameState.GameObjects[index] = tileReplacement;
            gameState.onPropertyChanged?.Invoke();
        }
    }
}
