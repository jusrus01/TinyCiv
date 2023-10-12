using TinyCiv.Client.Code.Decorators;

namespace TinyCiv.Client.Code.UnitDecorators
{
    public abstract class BorderDecorator : IBorderDecorator
    {
        protected GameObject wrappee;

        public BorderDecorator(GameObject gameObject)
        {
            wrappee = gameObject;
        }

        public void ApplyBorderEffects()
        {
            wrappee.ApplyBorderEffects();
        }

        public void RemoveBorderEffects()
        {
            wrappee?.RemoveBorderEffects();
        }
    }
}
