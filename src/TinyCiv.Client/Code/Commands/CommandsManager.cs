using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyCiv.Client.Code.Commands
{
    public class CommandsManager
    {
        private Queue<(IGameCommand Command, CancellationTokenSource CancellationTokenSource)> commandQueue = new Queue<(IGameCommand, CancellationTokenSource)>();
        private Stack<Queue<(IGameCommand, CancellationTokenSource)>> stateHistory = new Stack<Queue<(IGameCommand, CancellationTokenSource)>>();

        public async Task ExecuteCommandWithTimer(IGameCommand command, long durationInMillis)
        {
            stateHistory.Push(new Queue<(IGameCommand, CancellationTokenSource)>(commandQueue));

            var cancellationTokenSource = new CancellationTokenSource();
            commandQueue.Enqueue((command, cancellationTokenSource));

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(durationInMillis), cancellationTokenSource.Token);
                command.Execute();
            }
            catch (OperationCanceledException) { }
        }

        public void UndoLastCommand()
        {
            if (commandQueue.Count > 0)
            {
                var (_, currentCancellationTokenSource) = commandQueue.Dequeue();
                currentCancellationTokenSource.Cancel();
                currentCancellationTokenSource.Dispose();

                if (stateHistory.Count > 0)
                {
                    var previousCommandQueue = stateHistory.Pop();
                    commandQueue = previousCommandQueue;
                }
            }
        }
    }
}
