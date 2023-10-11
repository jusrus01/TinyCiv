using TinyCiv.Client.Code.Units;

namespace TinyCiv.Client.Code.UnitDecorators
{
    public abstract class UnitDecorator : Unit
    {
        protected Unit wrappee;

        public UnitDecorator(Unit unit) : base(unit)
        {
            wrappee = unit;
        }

        public void SetUnit(Unit unit)
        {
            wrappee = unit;
        }
    }
}
