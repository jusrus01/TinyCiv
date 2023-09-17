using TinyCiv.Server.Client;

namespace TinyCiv.Client.Code
{
    public class ClientManager
    {
        private static ClientManager instance;
        public IServerClient Client { get; }
        public bool requiresUpdate { get; set; }
        private ClientManager()
        {
            Client = ServerClient.Create("http://localhost:5000");
            requiresUpdate = false;
        }

        public static ClientManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientManager();
                }
                return instance;
            }
        }
    }
}
