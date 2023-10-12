using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code.UnitDecorators
{
    public class HealthBonusDecorator : UnitDecorator
    {
        public override int MaxHealth => (int)(wrappee.MaxHealth + wrappee.MaxHealth * 0.1);

        public HealthBonusDecorator(Unit unit) : base(unit)
        {
        }
    }
}
