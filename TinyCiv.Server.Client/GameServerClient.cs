using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TinyCiv.Shared;
using TinyCiv.Shared.Events;

namespace TinyCiv.Server.Client;

// Client should use this as a singleton,
// unclear how WPF is used, so allowing creation of client without DI
public class GameServerClient : IGameServerClient
{
    private readonly HubConnection _connection;
    
    private GameServerClient(HubConnection connection)
    {
        _connection = connection;
    }
    
    private static IGameServerClient? _client;
    
    public static IGameServerClient Create(string hostUrl)
    {
        if (_client != null)
        {
            return _client;
        }
        
        // Please modify according to your needs e.g.
        // move this or configure other way
        var connection = new HubConnectionBuilder()
            .WithUrl($"{hostUrl}{Constants.Server.HubRoute}")
            .AddJsonProtocol()
            .Build();

        connection.StartAsync().Wait();

        _client = new GameServerClient(connection);
        return _client;
    }
    
    public Task SendAsync(SimpleEvent @event, CancellationToken token)
    {
        return _connection.SendAsync(Constants.Server.ReceiveFromClient, @event, token);
    }

    public void AddListener(Action<SimpleEvent?> callback)
    {
        _connection.On<string>(
            Constants.Server.SendToClient, 
            (message) => callback(JsonSerializer.Deserialize<SimpleEvent?>(message)));
    }
}