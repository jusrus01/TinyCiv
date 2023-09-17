// NOTE: Need to run TinyCiv.Server
// IMPORTANT: DO NOT REFERENCE ANYTHING FROM "server" folder in the client and vice versa

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
    
    int? playerId1 = null;
    int? playerId2 = null;

    //bool gameStarted = false;
    
    Action<JoinLobbyServerEvent> joinCallback = (response) =>
    {
        if (playerId1 == null)
        {
            Console.WriteLine("Player 1 received id");
            playerId1 = response.NewPlayer.Id;
        }
        else if (playerId2 == null)
        {
            Console.WriteLine("Player 2 received id");
            playerId2 = response.NewPlayer.Id;
        }
        else
        {
            throw new Exception();
        }
    };

    async Task StartGameLoop()
    {
        // player (this instance)
        await client.SendAsync(new AddNewUnitClientEvent(playerId1.Value, 1, 1));
    }

    Action<GameStartServerEvent> startCallback = (response) =>
    {
        Console.WriteLine("Game started since two players already joined");
        Console.WriteLine($"Map to render: {response.Map}");
        
        // start some game loop, etc, probably best via delegate
        StartGameLoop().GetAwaiter();
    };
    
    Action<MapChangeServerEvent> mapChangeCallback = (response) =>
    {
        Console.WriteLine("Map changed since something happened...");
        Console.WriteLine($"Map to render: {response.Map}");
    };

    client.ListenForPlayerIdAssignment(joinCallback);
    client.ListenForGameStart(startCallback);
    
    client.ListenForMapChange(mapChangeCallback);
    
    // first player joins
    await client.SendAsync(new JoinLobbyClientEvent());
    
    // second player joins
    await client.SendAsync(new JoinLobbyClientEvent());
}