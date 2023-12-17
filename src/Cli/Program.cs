using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

const bool enableInfoLogging = true;

const int numberOfConnections = 300;
const int fiveMinutesInMilliseconds = 300000;
// const int fiveMinutesInMilliseconds = 10000;
const int delayInMilliseconds = 1000;

const string hostUrl = "http://localhost:5001";

var random = new Random();

var diagnostics = new ConcurrentDictionary<int, List<(double ResponseTime, double ResponseThroughput)>>();
var connections = new List<ServerClientV2>();

// Initialize connections to the server
await DoStepAsync(
    $"Trying to create {numberOfConnections} connections",
    async () =>
    {
        var connectTasks = new List<Task>();
        for (var i = 0; i < numberOfConnections; i++)
        {
            diagnostics.TryAdd(i, new List<(double ResponseTime, double ResponseThroughput)>());            
            
            var client = new ServerClientV2();
            connections.Add(client);
            
            var connectTask = client.ConnectAsync(hostUrl);
            connectTasks.Add(connectTask);
        }

        await Task.WhenAll(connectTasks);
    });

// Simulate users
await DoStepAsync(
    "Simulating users",
    async () =>
    {
        var cancellationTokenSource = new CancellationTokenSource();
        for (var i = 0; i < numberOfConnections; i++)
        {
            var threadId = i;
            var client = connections[i];
            
            var thread = new Thread(() => SimulateUserAsync(threadId, diagnostics[threadId], client, cancellationTokenSource.Token));
            thread.Start();
        }
        Log($"Initialized {numberOfConnections} threads. Will wait for '{fiveMinutesInMilliseconds}' ms before stopping execution");
        
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        
        // Block main thread for five minutes
        while (stopWatch.ElapsedMilliseconds <= fiveMinutesInMilliseconds)
        {
            await Task.Delay(delayInMilliseconds).ConfigureAwait(false);
        }

        cancellationTokenSource.Cancel();
        Log($"Stopping {numberOfConnections} threads");
        stopWatch.Stop();
        
        // allow to finish jobs
        await Task.Delay(10000).ConfigureAwait(false);
    });

// Log stuff
// foreach (var thread in diagnostics)
// {
//     Console.WriteLine($"thread: {thread.Key}");
//     foreach (var val in thread.Value)
//     {
//         if (val.ResponseTime < 0 && val.ResponseThroughput < 0)
//         {
//             Console.WriteLine($"packet loss");
//         }
//         else
//         {
//             Console.WriteLine($"response time: {val.ResponseTime} seconds, network: {val.ResponseThroughput} bps");
//         }
//     }
// }

// Create result file
var builder = new StringBuilder();
builder.AppendLine($"Connections made: '{numberOfConnections}'");
builder.AppendLine($"Test time: '{fiveMinutesInMilliseconds / 1000}' seconds");
builder.AppendLine($"Information about each client:");

int totalCount = 0;
double totalAvgResponseTime = 0;
double totalAvgResponseThroughput = 0;

foreach (var thread in diagnostics)
{
    var packetLossCount = 0;
    var actuallyProccesedRequests = 0;
    double localResponseTimeInSeconds = 0;
    double localResponseThroughput = 0;
  
    foreach (var val in thread.Value)
    {
        if (val.ResponseTime < 0 && val.ResponseThroughput < 0)
        {
            packetLossCount++;
            continue;
        }

        localResponseTimeInSeconds += val.ResponseTime;
        localResponseThroughput += val.ResponseThroughput;
        
        actuallyProccesedRequests++;
    }

    double avgResponseTime = actuallyProccesedRequests > 0 ? localResponseTimeInSeconds / actuallyProccesedRequests : 0;
    double avgResponseThroughput = actuallyProccesedRequests > 0 ? localResponseThroughput / actuallyProccesedRequests : 0;
                             builder.AppendLine($"    Server client id: '{thread.Key}'");
    builder.AppendLine($"        Lost packets: '{packetLossCount}'");
    builder.AppendLine($"        Average response time: '{avgResponseTime}' seconds");
    builder.AppendLine($"        Average network throughput: '{avgResponseThroughput}' bps");
    builder.AppendLine($"        Total packets sent: '{actuallyProccesedRequests + packetLossCount}'");

    totalAvgResponseTime += avgResponseTime;
    totalAvgResponseThroughput += avgResponseThroughput;
    totalCount++;
}
builder.AppendLine($"Average total response time: '{totalAvgResponseTime / totalCount}' seconds");
builder.AppendLine($"Average network throughput: '{totalAvgResponseThroughput / totalCount}' bps");

File.WriteAllText("result.txt", builder.ToString());

Log("DONE");
Console.ReadKey();

async void SimulateUserAsync(
    int threadId,
    ICollection<(double ResponseTime, double ResponseThroughput)> diagnosticInfo,
    ServerClientV2 client,
    CancellationToken token)
{
    var dirtyLocker = new object();
    var dirties = new List<Dirty>();

    client.ListenForUnitStatusUpdate(Measure);
    client.ListenForNewPlayerCreation(Measure);
    client.ListenForNewUnitCreation(Measure);
    client.ListenForGameStart(Measure);
    client.ListenForMapChange(Measure);
    client.ListenForResourcesUpdate(Measure);
    client.ListenForLobbyState(Measure);
    client.ListenForInteractableObjectChanges(Measure);
    client.ListenForGameModeChangeEvent(Measure);

    while (!token.IsCancellationRequested)
    {
        try
        {
            var @event = GetRandomEvent();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            lock (dirtyLocker)
            {
                dirties.Add(new Dirty
                {
                    Watch = stopWatch,
                    Type = nameof(GameStartServerEvent),
                    ConnectionId = client?.GetId(),
                    BytesSent = Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(@event))
                });
            }

            await client.SendAsync(@event).ConfigureAwait(false);
            await Task.Delay(random.Next(1000, 10000)).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            LogInfo($"Connection dropped from '{threadId}' thread, reason: {e.Message}. Reconnecting...");
            await client.ConnectAsync(hostUrl);
            
            client.ListenForUnitStatusUpdate(Measure);
            client.ListenForNewPlayerCreation(Measure);
            client.ListenForNewUnitCreation(Measure);
            client.ListenForGameStart(Measure);
            client.ListenForMapChange(Measure);
            client.ListenForResourcesUpdate(Measure);
            client.ListenForLobbyState(Measure);
            client.ListenForInteractableObjectChanges(Measure);
            client.ListenForGameModeChangeEvent(Measure);
            
            LogInfo($"Connection restored from '{threadId}' thread");
        }
    }

    await Task.Delay(5000).ConfigureAwait(false);
    
    foreach (var lostPackets in dirties)
    {
        diagnosticInfo.Add((-1, -1));
    }
    
    void Measure(ServerEvent @event)
    {
        Dirty dirty = null;
        lock (dirtyLocker)
        {
            dirty = dirties.FirstOrDefault(i => i.Type == @event.Type && i.ConnectionId == @event.ConnectionId);
        }
        
        if (dirty == null)
        {
            return;
        }

        var responseTimeInMs = dirty.Watch.ElapsedMilliseconds;

        var bytesReceived =  Encoding.UTF8.GetByteCount(JsonSerializer.Serialize(@event));
        
        // https://www.networkingsignal.com/what-is-throughput-in-networking/
        var networkThroughput = ((bytesReceived + dirty.BytesSent) / (responseTimeInMs / 1000)) * 8; // bps
        
        diagnosticInfo.Add((responseTimeInMs / 1000, networkThroughput));
        
        lock (dirtyLocker)
        {
            dirties.Remove(dirty);
        }
    }
}

ClientEvent GetRandomEvent()
{
    const int eventCount = 1;
    var eventIndex = random.Next(0, eventCount);

    return eventIndex switch
    {
        0 => new StartGameClientEvent(),
        1 => new AttackUnitClientEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
        2 => new ChangeGameModeClientEvent(Guid.NewGuid(), GameModeType.Normal),
        3 => new CreateBuildingClientEvent(Guid.NewGuid(), BuildingType.Bank, new ServerPosition { X = 10, Y = 10 }),
        4 => new CreateUnitClientEvent(Guid.NewGuid(), 10, 10),
        5 => new InterpretClientEvent(Guid.NewGuid(), "warrior atk warrior red"),
        6 => new MoveUnitClientEvent(Guid.NewGuid(), Guid.NewGuid(), 10, 10),
        7 => new PlaceTownClientEvent(Guid.NewGuid()),
        8 => new StartGameClientEvent(),
        _ => throw new Exception()
    };
}

async Task DoStepAsync(string message, Func<Task> taskFunc)
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    
    Log(message);
    await taskFunc();
    
    stopWatch.Stop();
    Log($"Done, took '{stopWatch.ElapsedMilliseconds}' ms");
}

void LogInfo(string message)
{
    if (enableInfoLogging)
    {
        Log(message);
    }
}

void Log(string message)
{
    Console.WriteLine(message);
}

class Dirty
{
    public string Type { get; set; }
    public Stopwatch Watch { get; set; }
    public string ConnectionId { get; set; }
    public int BytesSent { get; set; }
}