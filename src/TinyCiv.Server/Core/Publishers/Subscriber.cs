using Microsoft.AspNetCore.SignalR;

namespace TinyCiv.Server.Core.Publishers;

public record Subscriber(string ConnectionId, IClientProxy Proxy);
