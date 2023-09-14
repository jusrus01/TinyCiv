using TinyCiv.Shared.Events;

namespace TinyCiv.Server.Client;

public interface IGameServerClient
{
    Task SendAsync(SimpleEvent @event, CancellationToken token);

    void AddListener(Action<SimpleEvent?> callback);
}

