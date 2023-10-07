using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Extensions;

public static class GameObjectExtensions
{
    public static bool IsInteractable(this ServerGameObject obj)
    {
        return obj.Type is GameObjectType.Warrior or GameObjectType.Cavalry or GameObjectType.Colonist or GameObjectType.Tarran;
    }
}