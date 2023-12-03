using System.Collections.Concurrent;
using TinyCiv.Server.Core.Iterators;
using TinyCiv.Server.Core.Publishers;

namespace TinyCiv.Server.Publishers;

public class PublisherStorage : IPublisherStorage
{
    private readonly ConcurrentDictionary<string, Subscriber> _subscribers = new();
    
    public void Add(Subscriber subscriber)
    {
        _subscribers.TryAdd(subscriber.ConnectionId ?? throw new InvalidOperationException(), subscriber);
    }

    public void Remove(Subscriber subscriber)
    {
        _subscribers.TryRemove(subscriber.ConnectionId ?? throw new InvalidOperationException(), out _);
    }

    public IIterator<Subscriber?> GetIterator()
    {
        return new PublisherIterator(_subscribers);
    }
}