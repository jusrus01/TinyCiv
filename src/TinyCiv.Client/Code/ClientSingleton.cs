using System.Threading;
using TinyCiv.Server.Client;

namespace TinyCiv.Client.Code.MVVM
{
    public class ClientSingleton
    {
        private static readonly ClientSingleton instance = new ClientSingleton();

        public static ClientSingleton Instance { get { return instance; } }

        public IServerClient serverClient;
        private ManualResetEvent initializationEvent = new ManualResetEvent(false);

        private ClientSingleton()
        {
            serverClient = ServerClient.Create("http://localhost:5000");
            initializationEvent.Set(); // Signal that initialization is complete
        }

        // Add a method to check if the initialization is complete
        public void WaitForInitialization()
        {
            initializationEvent.WaitOne();
        }
    }
}
