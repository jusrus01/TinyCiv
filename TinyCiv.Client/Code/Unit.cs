using System;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TinyCiv.Client.Code
{
    public class Unit : GameObject
    {
        public TimeSpan MoveSpeed = new TimeSpan(0,0,1);
        DispatcherTimer moveChargeUp;
        public Action onUpdate;
        public Position target;

        public Unit(int r, int c)
        {
            imageSource = new BitmapImage(new Uri("Assets/warrior.png", UriKind.Relative));
            position = new Position(r, c);
        }

        public void moveTowards(Position target)
        {
            if (moveChargeUp != null)
            {
                moveChargeUp.Stop();
            }
            moveChargeUp = new DispatcherTimer();
            this.target = target;
            moveChargeUp.Interval = MoveSpeed;
            moveChargeUp.Tick += moveTowardsTarget;
            moveChargeUp.Start();
        }

        private void moveTowardsTarget(object sender, EventArgs e)
        {
            var delta = this.target - this.position;
            position += delta.Direction();
            onUpdate?.Invoke();
            if (position==target)
            {
                moveChargeUp.Stop();
                moveChargeUp = new DispatcherTimer();
            }
        }

    }
}
