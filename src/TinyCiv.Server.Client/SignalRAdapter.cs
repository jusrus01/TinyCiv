using TinyCiv.Shared;
using TinyCiv.Shared.Events.Server;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Connections.Client;

namespace TinyCiv.Server.Client;

internal class SignalRAdapter : IHubConnection
{
    private readonly HubConnection _connection;

    public SignalRAdapter(string hostUrl, Action<HttpConnectionOptions>? configureHttpConnection = null)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl($"{hostUrl}{Constants.Server.HubRoute}", configureHttpConnection!)
            .AddJsonProtocol()
            .Build();

        _connection.StartAsync().Wait();
    }

    public Task SendAsync(string methodName, object? obj1, object? obj2, CancellationToken token)
    {
        return _connection.SendAsync(methodName, obj1, obj2, token);
    }

    public IDisposable Listen(string methodName, Action<ServerEvent> callback)
    {
        return _connection.On<string, string>(
            methodName,
            (content, type) =>
            {
                if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(type))
                {
                    return;
                }

                var @event = ServerHelper.ResolveEvent(content, type);

                callback?.Invoke(@event);
            });
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}
