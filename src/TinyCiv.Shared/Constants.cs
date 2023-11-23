namespace TinyCiv.Shared;

public static class Constants
{
    public static class Game
    {
        public static class Interactable
        {
            public const int AttackIntervalInMilliseconds = 2000;

            public static class City
            {
                public const int InitialHealth = 200;
                public const int Damage = 20;
            }
            
            public static class Warrior
            {
                public const int InitialHealth = 40;
                public const int Damage = 20;
                public const int TarranDamageReductionMultiplier = 2;
                public const int Price = 50;
            }

            public static class Cavalry
            {
                public const int InitialHealth = 60;
                public const int Damage = 30;
                public const int LifeStealPercentage = 5;
                public const int Price = 100;
            }

            public static class Tarran
            {
                public const int InitialHealth = 60;
                public const int Damage = 10;
                public const int BuildingAttackMultiplier = 5;
                public const int Price = 50;
                public const int SpawnClonesBeforeDeath = 2;
            }
        }
        
        public const int MaxPlayerCount = 4;
        public const int MinPlayerCount = 2;

        public const int WidthSquareCount = 20;
        public const int HeightSquareCount = 20;

        public const int MovementSpeedMs = 500;
        public const int GameModeAbilityDurationMs = 5000;

        public const int BuildingSpaceFromTown = 3;
        public const int TownSpaceFromTown = 5;

        public const int FarmInterval = 5000;
        public const int MineInterval = 4500;
        public const int ShopInterval = 7000;
        public const int PortInterval = 4000;
        public const int BankInterval = 3000;
        public const int BlacksmithInterval = 6000;

        public const int FarmPrice = 50;
        public const int MinePrice = 50;
        public const int ShopPrice = 50;
        public const int PortPrice = 50;
        public const int BankPrice = 100;
        public const int BlacksmithPrice = 100;

        public const int StartingIndustry = 200;
        public const int StartingFood = 100;
        public const int StartingGold = 50;
    }
    
    public static class Server
    {
        public const string HubRoute = "/server";
        public const string ReceiveFromClient = nameof(ReceiveFromClient);

        public const string SendCreatedPlayer = nameof(SendCreatedPlayer);
        public const string SendCreatedUnit = nameof(SendCreatedUnit);
        public const string SendUnitStatusUpdate = nameof(SendUnitStatusUpdate);
        public const string SendResourcesStatusUpdate = nameof(SendResourcesStatusUpdate);

        public const string SendInteractableObjectChangesToAll = nameof(SendInteractableObjectChangesToAll);
        public const string SendLobbyStateToAll = nameof(SendLobbyStateToAll);
        public const string SendGameStartToAll = nameof(SendGameStartToAll);
        public const string SendMapChangeToAll = nameof(SendMapChangeToAll);
        public const string SendVictoryEventToAll = nameof(SendVictoryEventToAll);
        public const string SendDefeatEventToAll = nameof(SendDefeatEventToAll);

        public const string SendGameModeChangeEventToAll = nameof(SendGameModeChangeEventToAll);
    }

    public static class Assets
    {
        public const string GameTile = "Assets/game_tile.png";
    }
}