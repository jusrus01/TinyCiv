using System.Windows.Controls;

namespace TinyCiv.Client.Code.BorderDecorators
{
    public interface IBorderObject
    {
        BorderProperties ApplyEffects();
        BorderProperties RemoveEffects();
    }
}
