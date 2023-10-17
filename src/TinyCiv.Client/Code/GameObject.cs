using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameObject : BorderObject
    {
        public GameObjectType Type { get; private set; }
        public override Position Position { get; set; }
        public Guid OwnerId { get; private set; }
        public Guid Id { get; private set; }
        public TeamColor Color { get; private set; }
        public Guid? OpponentId { get; private set; }

        public string ImageSource { get; set; }
        public BorderProperties Border {  get; set; }
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
            Border = new BorderProperties();
            Border.Opacity = 1;
        }

        public GameObject(GameObjectType type, Position position, TeamColor color, double Opacity = 1)
        {
            Type = type;
            Position = position;
            Color = color;
            ImageSource = AbstractGameObjectFactory.getGameObjectImage(Color, Type);
            Border = new BorderProperties();
            Border.Opacity = Opacity;
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
            go.Border = new BorderProperties();
            go.Border.BackgroundBrush = Brushes.Transparent;
            return go;
        }

        public override BorderProperties ApplyEffects()
        {
            Border.BorderBrush = Brushes.Black;
            return Border;
        }

        public override BorderProperties RemoveEffects()
        {
            Border.BorderThickness = new Thickness(0);
            Border.BackgroundBrush = Brushes.Transparent;

            return Border;
        }
    }
}
