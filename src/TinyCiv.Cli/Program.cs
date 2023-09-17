// NOTE: Need to run TinyCiv.Server
// IMPORTANT: DO NOT REFERENCE ANYTHING FROM "server" folder in the client and vice versa

using System.Text.Json;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;

// await SampleCommunicationDemo();
await SampleIdAssignmentDemo();

Console.ReadKey();

// For now game will start after two clients connect,
// no validation for unique connections yet, so demo works in the same client instance...
async Task SampleIdAssignmentDemo()
{
    var client = ServerClient.Create("http://localhost:5000");
    
    Action<JoinLobbyServerEvent> joinCallback = (response) =>
    {
        Console.WriteLine($"Player created: {JsonSerializer.Serialize(response)}");
    };

    // NOT READY
    // async Task StartGameLoop()
    // {
    //     // player (this instance)
    //     await client.SendAsync(new AddNewUnitClientEvent(playerId1.Value, 1, 1));
    // }

    Action<GameStartServerEvent> startCallback = (response) =>
    {
        Console.WriteLine("Game started since two players already joined");
        Console.WriteLine($"Map to render: {JsonSerializer.Serialize(response)}");
        
        // start some game loop, etc, probably best via delegate
        // NOT READY
        // StartGameLoop().GetAwaiter();
    };
    
    // NOT READY
    Action<MapChangeServerEvent> mapChangeCallback = (response) =>
    {
        Console.WriteLine("Map changed since something happened...");
        Console.WriteLine($"Map to render: {JsonSerializer.Serialize(response)}");
    };

    client.ListenForNewPlayerCreation(joinCallback);
    client.ListenForGameStart(startCallback);
    
    client.ListenForMapChange(mapChangeCallback);
    
    // first player joins
    var p1 = client.SendAsync(new JoinLobbyClientEvent());
    // second player joins
    var p2 = client.SendAsync(new JoinLobbyClientEvent());

    await Task.WhenAll(p1, p2);
}