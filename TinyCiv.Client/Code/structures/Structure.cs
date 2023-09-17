using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.structures
{
    public abstract class Structure : GameObject
    {
        protected Structure(ServerGameObject serverGameObject) : base(serverGameObject)
        {
        }
    }
}
