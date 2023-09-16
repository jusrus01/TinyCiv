using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Client;

// Client should use this as a singleton,
// unclear how WPF is used, so allowing creation of client without DI
public class ServerClient : IServerClient
{
    private readonly HubConnection _connection;
    
    private ServerClient(HubConnection connection)
    {
        _connection = connection;
    }
    
    private static IServerClient? _client;
    
    public static IServerClient Create(string hostUrl)
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

        _client = new ServerClient(connection);
        return _client;
    }
    
    public Task SendAsync<T>(T @event, CancellationToken token = default) where T : ClientEvent
    {
        var content = JsonSerializer.Serialize(@event);
        return _connection.SendAsync(Constants.Server.ReceiveFromClient, content, @event.Type, token);
    }

    public void ListenForPlayerIdAssignment(Action<JoinLobbyServerEvent> callback)
    {
        Listen(Constants.Server.SendGeneratedId, callback);
    }

    public void ListenForGameStart(Action<GameStartServerEvent> callback)
    {
        Listen(Constants.Server.SendGameStartToAll, callback);
    }

    public void ListenForMapChange(Action<MapChangeServerEvent> callback)
    {
        Listen(Constants.Server.SendMapChangeToAll, callback);
    }

    private void Listen<T>(string methodName, Action<T> callback) where T : ServerEvent
    {
        _connection.On<string, string>(
            methodName,
            (content, type) =>
            {
                if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(type))
                {
                    return;
                }

                var @event = ResolveEvent(content, type);
                
                callback((T)@event);
            });
    }

    private static ServerEvent ResolveEvent(string content, string type)
    {
        if (type == nameof(JoinLobbyServerEvent))
        {
            return JsonSerializer.Deserialize<JoinLobbyServerEvent>(content)!;
        }

        if (type == nameof(GameStartServerEvent))
        {
            return JsonSerializer.Deserialize<GameStartServerEvent>(content)!;
        }
        
        if (type == nameof(MapChangeServerEvent))
        {
            return JsonSerializer.Deserialize<MapChangeServerEvent>(content)!;
        }
        
        throw new NotSupportedException();
    }
}