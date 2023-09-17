using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TinyCiv.Client.Code.units
{
    public abstract class Unit : GameObject
    {
        public TimeSpan MoveSpeed = new TimeSpan(0, 0, 1);
        DispatcherTimer moveChargeUp;
        public Action onUpdate;
        public Position target;

        protected Unit(int playerId, int r, int c) : base(playerId, r, c)
        {

        }

        public virtual void moveTowards(Position target)
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
            var delta = target - position;
            position += delta.Direction();
            onUpdate?.Invoke();
            if (position == target)
            {
                moveChargeUp.Stop();
                moveChargeUp = new DispatcherTimer();
            }
        }

    }
}
