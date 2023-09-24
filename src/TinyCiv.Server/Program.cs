using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Handlers;
using TinyCiv.Server.Hubs;
using TinyCiv.Server.Services;
using TinyCiv.Shared;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IMapService, MapService>();

builder.Services.AddTransient<IClientHandler, UnitMoveHandler>();
builder.Services.AddTransient<IClientHandler, UnitAddHandler>();
builder.Services.AddTransient<IClientHandler, LobbyHandler>();
builder.Services.AddTransient<IClientHandler, GameStartHandler>();

builder.Services
    .AddSignalR()
    .AddHubOptions<ServerHub>(options => options.EnableDetailedErrors = true)
    .AddJsonProtocol();

var app = builder.Build();

// app.UseAuthorization();
app.MapHub<ServerHub>(Constants.Server.HubRoute);

app.Run();

// Needed for integration tests, do not remove
namespace TinyCiv.Server
{
    public partial class Program { }
}