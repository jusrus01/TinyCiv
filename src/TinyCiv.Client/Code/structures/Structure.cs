using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.Structures
{
    public abstract class Structure : GameObject
    {
        public Structure(GameObject go) : base(go.Type, go.Position, go.OwnerId, go.Id, go.Color, go.OpponentId) { }
    }
}
