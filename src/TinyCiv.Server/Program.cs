using TinyCiv.Server.Core.Handlers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Handlers.Client;
using TinyCiv.Server.Hubs;
using TinyCiv.Server.Services;
using TinyCiv.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISessionService, DeprecatedSessionService>();

builder.Services.AddTransient<IClientHandler, UnitAddHandler>();
builder.Services.AddTransient<IClientHandler, LobbyHandler>();

builder.Services
    .AddSignalR()
    .AddHubOptions<ServerHub>(options => options.EnableDetailedErrors = true)
    .AddJsonProtocol();

var app = builder.Build();

// app.UseAuthorization();
app.MapHub<ServerHub>(Constants.Server.HubRoute);

app.Run();