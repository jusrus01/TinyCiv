using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM.Model;

namespace TinyCiv.Client.Code.Commands
{
    public interface ICommandInvokerMemento
    {
        LinkedList<CommandQueueModel> GetCommandQueue();
        bool GetIsExecuting();
    }
}
