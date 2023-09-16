using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events;

namespace TinyCiv.Client.Code
{
    public class EventHandler
    {
        public static EventHandler instance;

        public IServerClient client;

        private EventHandler()
        {
            client = ServerClient.Create("http://localhost:5000");
        }

        public static EventHandler GetInstance()
        {
            if (instance == null)
            {
                instance = new EventHandler();
            }
            return instance;
        }
    }
}
