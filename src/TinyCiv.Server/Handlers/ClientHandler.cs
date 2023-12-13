using System.Text.Json;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Middleware;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Handlers;

public abstract class ClientHandler<TEvent> : IClientHandler
{
    private const string HandleMethodName = nameof(HandleAsync);

    private readonly ILogger<IClientHandler> _logger;
    private readonly IPublisher _publisher;

    protected readonly IGameService GameService;

    private Subscriber? _caller;

    protected ClientHandler(IPublisher publisher, IGameService gameService, ILogger<IClientHandler> logger)
    {
        _logger = logger;
        _publisher = publisher;

        GameService = gameService;
    }

    public bool CanHandle(string type)
    {
        return type == typeof(TEvent).Name;
    }

    public Task HandleAsync(Subscriber subscriber, string eventContent)
    {
        ArgumentNullException.ThrowIfNull(eventContent);
        
        _caller = subscriber; // Simplest way to retrieve caller, trying to retrieve from DI is more complicated

        var handlerName = GetType().Name;

        var @event = JsonSerializer.Deserialize<TEvent>(eventContent)!;
        _logger.LogInformation("{handler}.{method_name}: deserialized event data {@data}", handlerName, HandleMethodName, @event);

        if (IgnoreWhen(@event))
        {
            _logger.LogWarning("{handler}.{method_name}: ignored event data {@data}", handlerName, HandleMethodName, @event);
            return Task.CompletedTask;
        }

        var loggerMiddleware = new LoggerMiddleware<TEvent>(_logger);
        var authorizationMiddleware = new AuthorizationMiddleware<TEvent>(GameService.GetMapService());
        var inputValidationMiddleware = new InputValidationMiddleware<TEvent>();
        var logicValidationMiddleware = new LogicValidationMiddleware<TEvent>(GameService.GetMapService());
        var stateMiddleware = new StateMiddleware<TEvent>(GameService.GetGameStateService());
        logicValidationMiddleware.SetNext(stateMiddleware);
        inputValidationMiddleware.SetNext(logicValidationMiddleware);
        authorizationMiddleware.SetNext(inputValidationMiddleware);
        loggerMiddleware.SetNext(authorizationMiddleware);

        bool result = loggerMiddleware.Handle(@event);

        if (result == false)
        {
            _logger.LogInformation("{handler}.{method_name}: failed to process event in middlewares '{@data}'", handlerName, HandleMethodName, @event);
            return Task.CompletedTask;
        }

        try
        {
            _logger.LogInformation("{handler}.{method_name}: starting to process event '{@data}'", handlerName, HandleMethodName, @event);
            return OnHandleAsync(@event);
        }
        catch
        {
            _logger.LogError("{handler}.{method_name}: failed to process event '{@data}'", handlerName, HandleMethodName, @event);
            throw;
        }
    }

    protected Task NotifyCallerAsync<T>( string methodName, T serverEvent) where T : ServerEvent
    {
        ArgumentNullException.ThrowIfNull(_caller);
        _logger.LogInformation("{handler} is sending event {event_type} to caller", GetType().Name, serverEvent.Type);
        return _publisher.NotifySubscriberAsync(_caller, methodName, serverEvent);
    }
    
    protected Task NotifyAllAsync<T>(string methodName, T serverEvent) where T : ServerEvent
    {
        _logger.LogInformation("{handler} is sending event {event_type} to all", GetType().Name, serverEvent.Type);
        return _publisher.NotifyAllAsync(methodName, serverEvent);
    }

    protected virtual bool IgnoreWhen(TEvent @event) => false;
        
    protected abstract Task OnHandleAsync(TEvent @event);
}