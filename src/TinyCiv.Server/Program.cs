using Serilog;
using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Handlers;
using TinyCiv.Server.Handlers.Lobby;
using TinyCiv.Server.Hubs;
using TinyCiv.Server.Services;
using TinyCiv.Shared.Events.Client;
using Constants = TinyCiv.Shared.Constants;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .Enrich.WithClientIp()
    .Enrich.WithCorrelationId()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddSingleton<IResourceService, ResourceService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IMapService, MapService>();
builder.Services.AddSingleton<IInteractableObjectService, InteractableObjectService>();
builder.Services.AddSingleton<ICombatService, CombatService>();

builder.Services.AddScoped<IConnectionIdAccessor, ConnectionIdAccessor>();

builder.Services.AddTransient<IMapReader, LocalFileMapReader>();
builder.Services.AddTransient<IMapLoader, MapLoader>();

builder.Services.AddTransient<IClientHandler, UnitMoveHandler>();
builder.Services.AddTransient<IClientHandler, UnitAddHandler>();
builder.Services.AddTransient<IClientHandler, JoinLobbyHandler>();
builder.Services.AddTransient<IClientHandler, LeaveLobbyHandler>();
builder.Services.AddTransient<IClientHandler, GameStartHandler>();
builder.Services.AddTransient<IClientHandler, UnitAttackHandler>();
builder.Services.AddTransient<IClientHandler, CreateBuildingHandler>();
builder.Services.AddTransient<IClientHandler, PlaceTownHandler>();

builder.Services
    .AddSignalR()
    .AddHubOptions<ServerHub>(options => options.EnableDetailedErrors = true)
    .AddJsonProtocol();

var app = builder.Build();

// app.UseAuthorization();
app.MapHub<ServerHub>(Constants.Server.HubRoute);

Log.Logger.Information("Application up and running");
app.Run();
Log.Logger.Information("Application stopped");

// Needed for integration tests, do not remove
namespace TinyCiv.Server
{
    public partial class Program { }
}