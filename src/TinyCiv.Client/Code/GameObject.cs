using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameObject
    {
        public GameObjectType Type { get; }
        public Position Position { get; set; }
        public Guid OwnerId { get; }
        public Guid Id { get; }
        public TeamColor Color { get; }

        public string ImageSource { get; set; }
        public Thickness Borderthickness { get; set; }
        public Action LeftAction { get; set; }
        public Action RightAction { get; set; }

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
