// NOTE: Need to run TinyCiv.Server
// IMPORTANT: DO NOT REFERENCE ANYTHING FROM "server" folder in the client and vice versa

using System.Text.Json;
using TinyCiv.Example;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Client.Lobby;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

// await SampleCommunicationDemo();
await SampleIdAssignmentDemo();

Console.ReadKey();

// For now game will start after two clients connect,
// no validation for unique connections yet, so demo works in the same client instance...
async Task SampleIdAssignmentDemo()
{
    var client = ServerClient.Create("http://localhost:5000");

    List<Player> playerList = new();
    List<ServerGameObject> gameObjects = new();
    List<Guid> movingUnits = new();

    Action<JoinLobbyServerEvent> joinCallback = (response) =>
    {
        Console.WriteLine($"Player created: {JsonSerializer.Serialize(response)}");
        playerList.Add(response.Created);
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
        //Console.WriteLine($"Map to render: {JsonSerializer.Serialize(response)}");

        response.Map.Print();

        // start some game loop, etc, probably best via delegate
        // NOT READY
        // StartGameLoop().GetAwaiter();
    };

    // NOT READY
    Action<MapChangeServerEvent> mapChangeCallback = (response) =>
    {
        Console.WriteLine("Map changed since something happened...");

        //Console.WriteLine($"Map to render: {JsonSerializer.Serialize(response)}");
        response.Map.Print();
    };

    Action<CreateUnitServerEvent> newUnitCallback = (response) =>
    {
        Console.WriteLine($"New unit created: {JsonSerializer.Serialize(response)}");
        gameObjects.Add(response.CreatedUnit);
    };

    Action<UnitStatusUpdateServerEvent> unitStatusUpdateCallback = (response) =>
    {
        Console.WriteLine($"Unit {response.UnitId} received a status update: IsMoving - {response.IsMoving}");

        if (response.IsMoving)
        {
            movingUnits.Add(response.UnitId);
        }
        else
        {
            movingUnits.Remove(response.UnitId);
        }
    };

    Action<ResourcesUpdateServerEvent> resourcesUpdateCallback = (response) =>
    {
        Console.WriteLine($"New resource status - {response.Resources.Gold}G, {response.Resources.Industry}I, {response.Resources.Food}F");
    };

    client.ListenForNewPlayerCreation(joinCallback);
    client.ListenForNewUnitCreation(newUnitCallback);
    client.ListenForGameStart(startCallback);
    client.ListenForMapChange(mapChangeCallback);
    client.ListenForUnitStatusUpdate(unitStatusUpdateCallback);
    client.ListenForResourcesUpdate(resourcesUpdateCallback);

    // first player joins
    var p1 = client.SendAsync(new JoinLobbyClientEvent());
    // second player joins
    var p2 = client.SendAsync(new JoinLobbyClientEvent());

    await Task.Delay(1000);

    await client.SendAsync(new StartGameClientEvent());

    await Task.Delay(1000);

    // Spawning 3 units
    await client.SendAsync(new CreateUnitClientEvent(playerList[0].Id, 1, 1));
    await client.SendAsync(new CreateUnitClientEvent(playerList[0].Id, 2, 2));
    await client.SendAsync(new CreateUnitClientEvent(playerList[1].Id, 5, 2));

    await Task.Delay(2000);

    // Moving units
    await client.SendAsync(new MoveUnitClientEvent(gameObjects[^1].Id, 10, 5));

    await Task.Delay(500);

    await client.SendAsync(new MoveUnitClientEvent(gameObjects[1].Id, 6, 7));
    await client.SendAsync(new MoveUnitClientEvent(gameObjects[0].Id, 6, 7)); // Should not work

    await client.SendAsync(new CreateBuildingClientEvent(playerList[0].Id, BuildingType.Blacksmith, new ServerPosition { X = 15, Y = 15 }));
    
    await Task.Delay(6000); // Blacksmith trigger

    await Task.Delay(500);

    await client.SendAsync(new CreateBuildingClientEvent(playerList[0].Id, BuildingType.Shop, new ServerPosition { X = 15, Y = 14 }));
    
    await Task.Delay(7000); // Mine trigger & blacksmith trigger

    await Task.Delay(1500); // Blacksmith trigger

    await Task.Delay(500);
}