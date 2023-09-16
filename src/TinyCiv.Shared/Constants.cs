namespace TinyCiv.Shared;

public static class Constants
{
    public static class Game
    {
        // Keep 2 for now, LobbyHandler unable to handle more complex logic ATM
        public const int MaxPlayerCount = 2;
        
        // Temporary map stuff, need to clarify how to best do this
        public const string Map =
            ".....\n" +
            ".....\n" +
            ".....\n" +
            ".....\n" +
            ".....\n";
    }
    
    public static class Server
    {
        public const string HubRoute = "/server";
        public const string ReceiveFromClient = "ReceiveFromClient";

        public const string SendGeneratedId = "SendGeneratedIdToClient";
        
        public const string SendGameStartToAll = "SendGameStartToAll";
        public const string SendMapChangeToAll = "SendMapChangeToAll";
    }
}