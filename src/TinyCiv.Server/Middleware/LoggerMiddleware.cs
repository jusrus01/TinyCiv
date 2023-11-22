using TinyCiv.Server.Core.Handlers;

namespace TinyCiv.Server.Middleware;

public class LoggerMiddleware<TEvent> : Core.Middlewares.Middleware<TEvent>
{
    private readonly ILogger<IClientHandler> _logger;

    public LoggerMiddleware(ILogger<IClientHandler> logger)
    {
        _logger = logger;
    }

    public override bool HandleInternal(TEvent @event)
    {
        var handlerName = GetType().Name;
        _logger.LogInformation("{handler}.{method_name}: starting the middlewares of event '{@data}'", handlerName, nameof(Handle), @event);
        return true;
    }
}
