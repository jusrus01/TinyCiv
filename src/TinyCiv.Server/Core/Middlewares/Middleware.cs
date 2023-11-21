namespace TinyCiv.Server.Core.Middlewares;

public abstract class Middleware<TEvent>
{
    protected Middleware<TEvent> next = null!;

    public void SetNext(Middleware<TEvent> next)
    {
        this.next = next;
    }

    public bool Handle(TEvent @event)
    {
        if (HandleInternal(@event) == false)
        {
            return false;
        }

        if (next != null)
        {
            return next.Handle(@event);
        }

        return true;
    }

    public abstract bool HandleInternal(TEvent @event);
}
