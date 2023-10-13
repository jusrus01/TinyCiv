using TinyCiv.Client.Code.Decorators;

namespace TinyCiv.Client.Code.UnitDecorators
{
    public abstract class BorderDecorator : BorderObject
    {
        protected BorderObject wrappee;

        public BorderDecorator(BorderObject borderObject)
        {
            wrappee = borderObject;
        }
        public override void ApplyBorderEffects() 
        {
            wrappee.ApplyBorderEffects();
        }
        public override void RemoveBorderEffects()
        {
            wrappee.RemoveBorderEffects();
        }

        public BorderObject GetWrappee() 
        { 
            return wrappee;
        }
    }
}
