using Serilog;
using TinyCiv.Server.BackgroundJobs;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Handlers;
using TinyCiv.Server.Handlers.Lobby;
using TinyCiv.Server.Hubs;
using TinyCiv.Server.Interpreter;
using TinyCiv.Server.Interpreter.Expressions;
using TinyCiv.Server.Publishers;
using TinyCiv.Server.Services;
using TinyCiv.Shared.Events.Client;
using Constants = TinyCiv.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

var enableLogging = Environment.GetEnvironmentVariable("DISABLE_LOGS") != "YES";
if (enableLogging)
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.Seq("http://localhost:5341")
        .Enrich.FromLogContext()
        .Enrich.WithClientIp()
        .Enrich.WithCorrelationId()
        .CreateLogger();
}
else
{
    Log.Logger = new LoggerConfiguration().CreateLogger();
}

builder.Host.UseSerilog();

builder.Services.AddSingleton<IPublisherStorage, PublisherStorage>();
builder.Services.AddSingleton<IPublisher, Publisher>();

builder.Services.AddSingleton<IResourceService, ResourceService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IMapService, MapService>();
builder.Services.AddSingleton<IInteractableObjectService, InteractableObjectService>();
builder.Services.AddSingleton<ICombatService, CombatService>();
builder.Services.AddSingleton<IGameStateService, GameStateService>();

builder.Services.AddScoped<IConnectionIdAccessor, ConnectionIdAccessor>();

builder.Services.AddTransient<IMapReader, LocalFileMapReader>();
builder.Services.AddTransient<IGameService,  GameService>();
builder.Services.AddTransient<IMapLoader, MapLoader>();

builder.Services.AddTransient<IExpressionStorage,  ExpressionStorage>();
builder.Services.AddTransient<IGameInterpreter, GameInterpreter>();

builder.Services.AddTransient<IClientHandler, UnitMoveHandler>();
builder.Services.AddTransient<IClientHandler, UnitAddHandler>();
builder.Services.AddTransient<IClientHandler, JoinLobbyHandler>();
builder.Services.AddTransient<IClientHandler, LeaveLobbyHandler>();
builder.Services.AddTransient<IClientHandler, GameStartHandler>();
builder.Services.AddTransient<IClientHandler, UnitAttackHandler>();
builder.Services.AddTransient<IClientHandler, CreateBuildingHandler>();
builder.Services.AddTransient<IClientHandler, PlaceTownHandler>();
builder.Services.AddTransient<IClientHandler, ChangeGameModeHandler>();
builder.Services.AddTransient<IClientHandler, InterpreterHandler>();

builder.Services.AddHostedService<GameBoardAnalyzerBackgroundJob>();

builder.Services
    .AddSignalR()
    .AddHubOptions<ServerHub>(options => options.EnableDetailedErrors = true)
    .AddJsonProtocol();

var app = builder.Build();

// app.UseAuthorization();
app.MapHub<ServerHub>(Constants.Server.HubRoute);

Log.Logger.Information($"Application up and running in env: '{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}'");
app.Run();
Log.Logger.Information("Application stopped");

// Needed for integration tests, do not remove
namespace TinyCiv.Server
{
    public partial class Program { }
}