using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Core.Publishers;

public interface IPublisher
{
    void Subscribe(Subscriber subscriber);
    void Unsubscribe(Subscriber subscriber);
    
    Task NotifyAllAsync<T>(string methodName, T serverEvent) where T : ServerEvent;
    Task NotifySubscriberAsync<T>(Subscriber subscriber, string methodName, T serverEvent) where T : ServerEvent;
}