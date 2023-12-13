namespace TinyCiv.Server.Core.Iterators;

public interface IIterator<T>
{
    T Next();
    bool HasNext();
}