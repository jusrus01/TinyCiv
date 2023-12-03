using System.Collections.Concurrent;
using TinyCiv.Server.Core.Iterators;
using TinyCiv.Server.Core.Publishers;

namespace TinyCiv.Server.Publishers;

public class PublisherIterator : IIterator<Subscriber?>
{
    private readonly ConcurrentDictionary<string, Subscriber> _subscribers;
    private readonly Queue<string> _keys;

    public PublisherIterator(ConcurrentDictionary<string, Subscriber> subscribers)
    {
        _subscribers = subscribers;

        _keys = new Queue<string>();
        foreach (var key in _subscribers.Keys)
        {
            _keys.Enqueue(key);
        }
    }
    
    public Subscriber? Next()
    {
        while (HasNext())
        {
            var key = _keys.Dequeue();
            if (_subscribers.TryGetValue(key, out var subscriber))
            {
                return subscriber;
            }
        }

        return null;
    }

    public bool HasNext()
    {
        return _keys.Any();
    }
}