using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code.UnitDecorators
{
    public class SpeedPenaltyDecorator : UnitDecorator
    {
        public override int Speed => (int)(wrappee.Speed - wrappee.Speed * 0.2);

        public SpeedPenaltyDecorator(Unit unit) : base(unit)
        {
        }
    }
}
