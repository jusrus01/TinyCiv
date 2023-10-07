using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Core.Extensions;

public static class GameObjectExtensions
{
    public static bool IsInteractable(this ServerGameObject obj)
    {
        return obj.Type == GameObjectType.Warrior;
    }
}