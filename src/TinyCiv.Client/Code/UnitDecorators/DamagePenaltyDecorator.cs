using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code.UnitDecorators
{
    public class DamagePenaltyDecorator : UnitDecorator
    {
        public override int Damage => (int)(wrappee.Damage - wrappee.Damage * 0.5);

        public DamagePenaltyDecorator(Unit unit) : base(unit)
        {
        }
    }
}
