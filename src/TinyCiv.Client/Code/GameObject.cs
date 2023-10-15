using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameObject : IBorderObject
    {
        public GameObjectType Type { get; private set; }
        public Position Position { get; set; }
        public Guid OwnerId { get; private set; }
        public Guid Id { get; private set; }
        public TeamColor Color { get; private set; }
        public Guid? OpponentId { get; private set; }

        public string ImageSource { get; set; }
        public BorderProperties Border {  get; set; }

        //public Border Border { get; set; }
        //public Thickness BorderThickness { get; set; }
        //public Brush BorderBrush { get; set; }
        //public Brush BackgroundBrush { get; set; }
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

        public BorderProperties ApplyEffects()
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
                Border.BorderBrush = Brushes.Black;
            //});
            return Border;
        }

        public BorderProperties RemoveEffects()
        {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
                Border.BorderThickness = new Thickness(0);
                Border.BackgroundBrush = Brushes.Transparent;
            //});

            return Border;
        }
    }
}
