using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TinyCiv.Client.Code
{
    public class GameObject
    {
        public ImageSource imageSource { get; protected set; }
        DateTime LastUpdate;
        TimeSpan TimeDelta;
        public Position position;

        public GameObject() { }
        public GameObject(BitmapImage imageSource, int r, int c) : this(imageSource, new Position(r, c)) { }
        public GameObject(BitmapImage imageSource, Position position)
        {
            this.imageSource = imageSource;
            this.position = position;
        }

        public void Start()
        {
            LastUpdate = DateTime.Now;
        }

        public void Update()
        {
            DateTime now = DateTime.Now;
            TimeDelta = now-LastUpdate;
            LastUpdate = now;
        }
    }
}
