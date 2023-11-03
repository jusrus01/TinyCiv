using System;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class CurrentPlayer
    {
        private static readonly CurrentPlayer instance = new CurrentPlayer();

        public static CurrentPlayer Instance { get { return instance; } }

        public static Guid Id { get { return instance.player.Id; } }

        public static TeamColor Color { get { return instance.player.Color; } }

        public Resources Resources { get; set; }
        public static bool IsOwner(GameObject gameObject)
        {
            return gameObject.OwnerId == Id;
        }

        //-------------------------

        public Player player;

        private CurrentPlayer() {}
    }
}
