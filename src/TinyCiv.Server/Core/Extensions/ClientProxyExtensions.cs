using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Core.Extensions;

public static class ClientProxyExtensions
{
    public static Task SendEventAsync<T>(this IClientProxy proxy, string methodName, T @event) where T : ServerEvent
    {
        var content = JsonSerializer.Serialize(@event);
        var type = @event.Type;

        return proxy.SendAsync(methodName, content, type);
    }
}