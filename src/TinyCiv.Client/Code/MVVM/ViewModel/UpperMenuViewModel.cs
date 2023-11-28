using System;
using System.Globalization;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Client;
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
        public ObservableValue<bool> IsPanicButtonEnabled { get; } = new ObservableValue<bool>(true);
        public ObservableValue<string> PanicModeText { get; } = new ObservableValue<string>("Off");

        public RelayCommand PanicButtonCommand => new RelayCommand(execute => TogglePanic());

        private bool _playerHasUsedPanic = false;

        private void TogglePanic()
        {
            if (_playerHasUsedPanic)
            {
                ClientSingleton.Instance.serverClient.SendAsync(new ChangeGameModeClientEvent(CurrentPlayer.Id, GameModeType.Normal));
                IsPanicButtonEnabled.Value = false;
            }
            else
            {
                ClientSingleton.Instance.serverClient.SendAsync(new ChangeGameModeClientEvent(CurrentPlayer.Id, GameModeType.RestrictedMode));
                _playerHasUsedPanic = true;
            }
        }

        public UpperMenuViewModel()
        {
            PlayerColor.Value = CurrentPlayer.Color;
            ClientSingleton.Instance.serverClient.ListenForResourcesUpdate(OnResourceUpdate);
            ClientSingleton.Instance.serverClient.ListenForGameModeChangeEvent(OnGameModeUpdate);
        }

        private void OnGameModeUpdate(GameModeChangeServerEvent response)
        {
            switch (response.GameModeType)
            {
                case GameModeType.Normal:
                    PanicModeText.Value = "Off";
                    break;
                case GameModeType.RestrictedMode:
                    PanicModeText.Value = "On";
                    break;
                default:
                    PanicModeText.Value = "Partial";
                    break;
            }
        }

        public void OnResourceUpdate(ResourcesUpdateServerEvent response)
        {
            var resources = response.Resources;
            Gold.Value = resources.Gold;
            Industry.Value = resources.Industry;
            Food.Value = resources.Food;
            CurrentPlayer.Instance.Resources = response.Resources;
        }
    }
}
