using System.Windows.Controls;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public abstract class BorderObject
    {
        public abstract Position Position { get; set; }
        public abstract BorderProperties ApplyEffects();
        public abstract BorderProperties RemoveEffects();
    }
}
