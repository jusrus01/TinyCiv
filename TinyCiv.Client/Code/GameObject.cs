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
        public int ownerId { get; protected set; }
        public ImageSource imageSource { get; protected set; }

        protected DateTime LastUpdate;
        protected TimeSpan TimeDelta;
        public Position position;

        public GameObject() { }
        public GameObject(int ownerId, ImageSource imageSource, int r, int c) : this(ownerId, imageSource, new Position(r, c)) { }
        public GameObject(int ownerId, ImageSource imageSource, Position position)
        {
            this.ownerId = ownerId;
            this.imageSource = imageSource;
            this.position = position;
        }

        public virtual void Start()
        {
            LastUpdate = DateTime.Now;
        }

        public virtual void Update()
        {
            DateTime now = DateTime.Now;
            TimeDelta = now-LastUpdate;
            LastUpdate = now;
        }
    }
}
