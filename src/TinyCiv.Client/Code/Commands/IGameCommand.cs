using System.Threading;
using System.Threading.Tasks;

namespace TinyCiv.Client.Code.Commands
{
    public interface IGameCommand
    {
        void Execute();
    }
}
