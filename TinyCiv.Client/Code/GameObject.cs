using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public abstract class GameObject
    {
        public GameObjectType Type { get; }
        public Position Position { get; set; }
        public Guid OwnerId { get; }
        public Guid Id { get; }
        public PlayerColor Color { get; }

        public GameObject(ServerGameObject serverGameObject)
        {
            Type = serverGameObject.Type;
            Position = new Position(serverGameObject.Position);
            OwnerId = serverGameObject.OwnerPlayerId;
            Id = serverGameObject.Id;
            Color = serverGameObject.Color;
        }
    }
}
