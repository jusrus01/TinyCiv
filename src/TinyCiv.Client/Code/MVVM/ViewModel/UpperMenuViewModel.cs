using TinyCiv.Shared;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class UpperMenuViewModel : ObservableObject
    {
        public ObservableValue<int> Industry { get; } = new ObservableValue<int>(Constants.Game.StartingIndustry);
        public ObservableValue<int> Gold { get; } = new ObservableValue<int>(Constants.Game.StartingGold);
        public ObservableValue<int> Food { get; } = new ObservableValue<int>(Constants.Game.StartingFood);
        public ObservableValue<TeamColor> PlayerColor { get; } = new ObservableValue<TeamColor>();

        public UpperMenuViewModel()
        {
            PlayerColor.Value = CurrentPlayer.Color;
            ClientSingleton.Instance.serverClient.ListenForResourcesUpdate(OnResourceUpdate);
        }

        public void OnResourceUpdate(ResourcesUpdateServerEvent response)
        {
            var resources = response.Resources;
            Gold.Value = resources.Gold;
            Industry.Value = resources.Industry;
            Food.Value = resources.Food;
        }
    }
}
