using TinyCiv.Server.Core.Iterators;

namespace TinyCiv.Server.Core.Publishers;

public interface IPublisherStorage
{
    void Add(Subscriber subscriber);
    void Remove(Subscriber subscriber);
    IIterator<Subscriber?> GetIterator();
}