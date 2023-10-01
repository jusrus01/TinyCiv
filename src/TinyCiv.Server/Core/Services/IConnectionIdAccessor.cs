using Microsoft.AspNetCore.SignalR;

namespace TinyCiv.Server.Core.Services;

public interface IConnectionIdAccessor
{
    public string ConnectionId { get; }

    void Init(HubCallerContext context);
}