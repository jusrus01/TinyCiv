using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Extensions;

public static class GameObjectExtensions
{
    public static bool IsInteractable(this ServerGameObject obj)
    {
        return IsInteractable(obj.Type);
    }

    public static bool IsInteractable(this GameObjectType type)
    {
        return type is GameObjectType.Warrior or GameObjectType.Cavalry or GameObjectType.Tarran;
    }
}