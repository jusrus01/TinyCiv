using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UpperMenuViewModel : ObservableObject
    {
        public ObservableValue<int> Industry { get; } = new ObservableValue<int>(Constants.Game.StartingIndustry);
        public ObservableValue<int> Gold { get; } = new ObservableValue<int>(Constants.Game.StartingGold);
        public ObservableValue<int> Food { get; } = new ObservableValue<int>(Constants.Game.StartingFood);
        public ObservableValue<TeamColor> PlayerColor { get; } = new ObservableValue<TeamColor>();

        public void SetResources(Resources resources)
        {
            Gold.Value = resources.Gold;
            Industry.Value = resources.Industry;
            Food.Value = resources.Food;
        }
    }
}
