using Microsoft.AspNetCore.Http.Connections.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Client;

internal interface IHubConnection
{
    Task SendAsync(string methodName, object? obj1, object? obj2, CancellationToken token);
    IDisposable Listen(string methodName, Action<ServerEvent> callback);
    ValueTask DisposeAsync();
}
