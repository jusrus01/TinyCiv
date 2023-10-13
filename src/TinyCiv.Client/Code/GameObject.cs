using System;
using System.Windows;
using System.Windows.Media;
using TinyCiv.Client.Code.Decorators;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameObject : BorderObject
    {
        public GameObjectType Type { get; private set; }
        public Position Position { get; set; }
        public Guid OwnerId { get; private set; }
        public Guid Id { get; private set; }
        public TeamColor Color { get; private set; }
        public Guid? OpponentId { get; private set; }

        public string ImageSource { get; set; }
        public override Thickness BorderThickness { get; set; }
        public override Brush BorderBrush { get; set; }
        public override Brush BackgroundBrush { get; set; }
        public Action LeftAction { get; set; }
        public Action RightAction { get; set; }
       

        public GameObject(GameObjectType type, Position position, Guid ownerId, Guid id, TeamColor color, Guid? opponentId)
        {
            Type = type;
            Position = position;
            OwnerId = ownerId;
            Id = id;
            Color = color;
            OpponentId = opponentId;
        }

        protected GameObject() { }

        public static GameObject fromServerGameObject(ServerGameObject serverGameObject)
        {
            var go = new GameObject();
            go.Type = serverGameObject.Type;
            go.Position = new Position(serverGameObject.Position);
            go.OwnerId = serverGameObject.OwnerPlayerId;
            go.Id = serverGameObject.Id;
            go.Color = serverGameObject.Color;
            go.OpponentId = serverGameObject.OpponentId;
            return go;
        }

        public override void ApplyBorderEffects()
        {
            BorderThickness = new Thickness(2);
        }

        public override void RemoveBorderEffects()
        {
            BorderThickness = new Thickness(0);
        }
    }
}
