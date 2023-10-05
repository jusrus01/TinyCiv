using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UpperMenuViewModel : ObservableObject
    {
        public ObservableValue<TeamColor> PlayerColor { get; } = new ObservableValue<TeamColor>();

    }
}
