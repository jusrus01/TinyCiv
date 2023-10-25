﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM.Model;

namespace TinyCiv.Client.Code.Commands
{
    public class CommandInvoker
    {
        private LinkedList<CommandQueueModel> commandQueue = new LinkedList<CommandQueueModel>();
        private bool isExecuting = false;

        public async Task AddCommandToQueue(IGameCommand command, long durationInMillis, Position position)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            commandQueue.AddLast(new CommandQueueModel(command, durationInMillis, cancellationTokenSource, position));

            if (!isExecuting)
            {
                await ExecuteNextCommand();
            }
        }

        private async Task ExecuteNextCommand()
        {
            if (commandQueue.Count > 0)
            {
                isExecuting = true;
                var commandTask = commandQueue.First();
                try
                {
                    if (commandTask.Command.CanExecute())
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(commandTask.Duration), commandTask.CancellationTokenSource.Token);
                        commandTask.Command.Execute();
                        commandQueue.RemoveFirst();
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    
                }
                catch (OperationCanceledException) { }

                if (commandQueue.Count == 0)
                {
                    isExecuting = false;
                }
                else
                {
                    await ExecuteNextCommand();
                }
            }
        }

        public CommandQueueModel UndoLastCommand()
        {
            if (commandQueue.Count > 0)
            {
                var commandTask = commandQueue.Last();
                commandTask.CancellationTokenSource.Cancel();
                commandTask.CancellationTokenSource.Dispose();
                commandQueue.RemoveLast();
                return commandTask;
            }
            return null;
        }
    }
}
