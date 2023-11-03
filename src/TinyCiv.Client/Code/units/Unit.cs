using System.Threading.Tasks;
using TinyCiv.Client.Code.MVVM;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Client.Code.Units;

public abstract class Unit : GameObject
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Damage { get; set; }
    public int Speed { get; set; }
    public int ProductionPrice { get; set; }
    public int ExpReward { get; set; }
    public string Description { get; set; }

    protected Unit() : base() { }

    protected Unit(GameObject go) : base(go.Type, go.Position, go.OwnerId, go.Id, go.Color, go.OpponentId)
    {

    }

    public async Task MoveTo(Position position)
    {
        await ClientSingleton.Instance.serverClient.SendAsync(new MoveUnitClientEvent(Id, position.row, position.column));
    }
}
