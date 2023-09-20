using System;
using System.Windows.Threading;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Units;

public abstract class Unit : GameObject
{
    public TimeSpan MoveSpeed = new TimeSpan(0, 0, 1);
    DispatcherTimer moveChargeUp;
    public Action onUpdate;
    public Position target;

    protected Unit(ServerGameObject serverGameObject) : base(serverGameObject)
    {
    }

    // Maybe transfer to server side?
    public virtual void MoveTowards(Position target)
    {
        if (moveChargeUp != null)
        {
            moveChargeUp.Stop();
        }
        moveChargeUp = new DispatcherTimer();
        this.target = target;
        moveChargeUp.Interval = MoveSpeed;
        moveChargeUp.Tick += MoveTowardsTarget;
        moveChargeUp.Start();
    }

    private void MoveTowardsTarget(object sender, EventArgs e)
    {
        var delta = target - Position;
        Position += delta.Direction();
        onUpdate?.Invoke();

        if (Position == target)
        {
            moveChargeUp.Stop();
            moveChargeUp = new DispatcherTimer();
        }
    }

}
