using TinyCiv.Server.Hubs;
using TinyCiv.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSignalR()
    .AddHubOptions<ServerHub>(options => options.EnableDetailedErrors = true)
    .AddJsonProtocol();

var app = builder.Build();

// app.UseAuthorization();
app.MapHub<ServerHub>(Constants.Server.HubRoute);

app.Run();