using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Interpreter;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Handlers;

public class InterpreterHandler : ClientHandler<InterpretClientEvent>
{
    private readonly IGameInterpreter _interpreter;
    private readonly ISessionService _sessionService;

    public InterpreterHandler(ISessionService sessionService, IGameInterpreter interpreter, IPublisher publisher, IGameService gameService, ILogger<IClientHandler> logger) : base(publisher, gameService, logger)
    {
        _interpreter = interpreter;
        _sessionService = sessionService;
    }

    protected override bool IgnoreWhen(InterpretClientEvent @event) => !_sessionService.IsStarted();

    protected override Task OnHandleAsync(InterpretClientEvent @event)
    {
        Task.Run(() => _interpreter.Interpret(@event.PlayerId, @event.Content));
        return Task.CompletedTask;
    }
}