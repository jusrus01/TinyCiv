using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TinyCiv.Client.Code
{
    public abstract class GameObject
    {
        public int playerId { get; protected set; }
        public abstract GameObjectType Type { get; }

        protected DateTime LastUpdate;
        protected TimeSpan TimeDelta;
        public Position position;

        protected GameObject() { }
        protected GameObject(int playerId, int r, int c) : this(playerId, new Position(r, c)) { }
        protected GameObject(int playerId, Position position)
        {
            this.playerId = playerId;
            this.position = position;
        }
    }
}
