using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Structures
{
    public abstract class Structure : GameObject
    {
        protected Structure(ServerGameObject serverGameObject) : base(serverGameObject)
        {
        }
    }
}
