using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic.CompilerServices;
using TinyCiv.Server.Core.Services;

namespace TinyCiv.Server.Services;

/// <summary>
/// Main idea - should have single instance per client connection,
/// that is why connection id once set should be the same on all following requests
/// </summary>
public class ConnectionIdAccessor : IConnectionIdAccessor
{
    private readonly ILogger<ConnectionIdAccessor> _logger;
    
    public ConnectionIdAccessor(ILogger<ConnectionIdAccessor> logger)
    {
        _logger = logger;
    }
    
    private string? _id;

    public string ConnectionId => _id ?? throw new IncompleteInitialization();

    public void Init(HubCallerContext context)
    {
        var connectionId = context.ConnectionId;
        
        if (_id != null && _id != connectionId)
        {
            _logger.LogError(
                "Connection instance received different connection id. Previous: '{prev}', received: '{cur}'",
                _id,
                connectionId);
            throw new InvalidOperationException();
        }

        _id = connectionId;
        _logger.LogInformation("Connection id set: {id}", _id);
    }
}