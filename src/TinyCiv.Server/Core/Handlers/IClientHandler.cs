using TinyCiv.Server.Core.Publishers;

namespace TinyCiv.Server.Core.Handlers;

public interface IClientHandler
{
    bool CanHandle(string type);
    
    Task HandleAsync(Subscriber subscriber, string eventContent);
}