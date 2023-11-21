using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TinyCiv.Client.Code.BorderDecorators;
using TinyCiv.Client.Code.Factories;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code
{
    public class GameObject : BorderObject
    {
        public virtual GameObjectType Type { get; private set; }
        public override Position Position { get; set; }
        public Guid OwnerId { get; set; }
        public Guid Id { get; set; }
        public TeamColor Color { get; set; }
        public Guid? OpponentId { get; set; }

        public IntrinsicGameobject intrinsic { get; set; }

        public Image ImageSource
        {
            get
            {
                return intrinsic.Image; 
            }
            set
            {
                intrinsic.Image = value;
            }
        }
        public BorderProperties Border {  get; set; }
        public Action LeftAction { get; set; }
        public Action RightAction { get; set; }       

        public GameObject(GameObjectType type, Position position, Guid ownerId, Guid id, TeamColor color, Guid? opponentId) 
            : this(type, position, color)
        {
            Id = id;
            OwnerId = ownerId;
            OpponentId = opponentId;
            intrinsic = new IntrinsicGameobject();
        }

        public GameObject(GameObjectType type, Position position, TeamColor color, double Opacity = 1) 
            : this(color, position, AbstractGameObjectFactory.getGameObjectImage(color, type))
        {
            Type = type;
            Border.Opacity = Opacity;
            intrinsic = new IntrinsicGameobject();
        }

        public GameObject(TeamColor color, Position position, Image image) : this()
        {
            Color = color;
            Position = position;
            intrinsic = new IntrinsicGameobject();
            intrinsic.Image = image;
        }

        protected GameObject() 
        {
            Border = new BorderProperties();
            intrinsic = new IntrinsicGameobject();
        }

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
            go.Border.Opacity = 1;
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
