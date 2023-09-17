namespace TinyCiv.Shared;

public static class Constants
{
    public static class Game
    {
        public const int MaxPlayerCount = 2;

        public const int WidthSquareCount = 20;
        public const int HeightSquareCount = 20;
        
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
        public const string ReceiveFromClient = nameof(ReceiveFromClient);

        public const string SendCreatedPlayer = nameof(SendCreatedPlayer);

        public const string SendGameStartToAll = nameof(SendGameStartToAll);
        public const string SendMapChangeToAll = nameof(SendMapChangeToAll);
    }
}