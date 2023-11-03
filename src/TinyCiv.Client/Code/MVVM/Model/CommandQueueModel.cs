using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Commands;

namespace TinyCiv.Client.Code.MVVM.Model
{
    public class CommandQueueModel
    {
        public IGameCommand Command;
        public long Duration;
        public CancellationTokenSource CancellationTokenSource;
        public Position Position;

        public CommandQueueModel(IGameCommand command, long duration, CancellationTokenSource cancellationTokenSource, Position position)
        {
            Command = command;
            Duration = duration;
            CancellationTokenSource = cancellationTokenSource;
            Position = position;
        }
    }
}
