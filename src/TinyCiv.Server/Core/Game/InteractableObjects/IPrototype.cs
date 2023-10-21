namespace TinyCiv.Server.Core.Game.InteractableObjects;

public interface IPrototype<out T> where T : class
{
    T Clone();
}