using Microsoft.AspNetCore.SignalR;

namespace TinyCiv.Server.Core.Handlers;

public interface IClientHandler
{
    bool CanHandle(string type);
    
    Task HandleAsync(IClientProxy caller, IClientProxy all, string eventContent);
}