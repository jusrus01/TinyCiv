namespace TinyCiv.Shared;

public static class Constants
{
    public static class Game
    {
        public const int MaxPlayerCount = 4;
        public const int MinPlayerCount = 2;

        public const int WidthSquareCount = 20;
        public const int HeightSquareCount = 20;

        public const int MovementSpeedMs = 1000;
        
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
        public const string SendCreatedUnit = nameof(SendCreatedUnit);
        public const string SendUnitStatusUpdate = nameof(SendUnitStatusUpdate);

        public const string SendLobbyStateToAll = nameof(SendLobbyStateToAll);
        public const string SendGameStartToAll = nameof(SendGameStartToAll);
        public const string SendMapChangeToAll = nameof(SendMapChangeToAll);
    }

    public static class Assets
    {
        public const string GameTile = "Assets/game_tile.png";
    }
}