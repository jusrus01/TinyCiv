// NOTE: Need to run TinyCiv.Server
// IMPORTANT: DO NOT REFERENCE ANYTHING FROM "server" folder in the client and vice versa

using TinyCiv.Server.Client;
using TinyCiv.Shared.Events;

// Sample code to reference how communication will work

// Create single instance of the client
var client = GameServerClient.Create("http://localhost:5000");

// Add listener that will respond to server messages
client.AddListener(ReceiveEventFromServer);

// Send message to client
await client.SendAsync(new SimpleEvent("Hello, server!"), CancellationToken.None);

void ReceiveEventFromServer(SimpleEvent? @event)
{
    Console.WriteLine(@event!.Message);
}

Console.ReadKey();