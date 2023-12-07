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
        public ObservableValue<bool> IsUnitOnlyButtonEnabled { get; } = new ObservableValue<bool>(true);
        public ObservableValue<bool> IsBuildingOnlyButtonEnabled { get; } = new ObservableValue<bool>(true);
        public ObservableValue<bool> IsRestrictedButtonEnabled { get; } = new ObservableValue<bool>(true);
        public ObservableValue<string> GameModeText { get; } = new ObservableValue<string>("NotStarted");

        public RelayCommand UnitOnlyButtonCommand => new RelayCommand(execute => ActivateUnitOnly());
        public RelayCommand BuildingOnlyButtonCommand => new RelayCommand(execute => ActivateBuildingOnly());
        public RelayCommand ResitrictedButtonCommand => new RelayCommand(execute => ActivateRestricted());

        private bool _playerHasUsedUnitOnly = false;
        private bool _playerHasUsedBuildingOnly = false;
        private bool _playerHasUsedRestricted = false;

        private void ActivateUnitOnly()
        {
            ClientSingleton.Instance.serverClient.SendAsync(new ChangeGameModeClientEvent(CurrentPlayer.Id, GameModeType.UnitOnly));
            _playerHasUsedUnitOnly = true;
        }

        private void ActivateBuildingOnly()
        {
            ClientSingleton.Instance.serverClient.SendAsync(new ChangeGameModeClientEvent(CurrentPlayer.Id, GameModeType.BuildingOnly));
            _playerHasUsedBuildingOnly = true;
        }

        private void ActivateRestricted()
        {
            ClientSingleton.Instance.serverClient.SendAsync(new ChangeGameModeClientEvent(CurrentPlayer.Id, GameModeType.RestrictedMode));
            _playerHasUsedRestricted = true;
        }

        private void SetAbilityButtonsState(bool isActive)
        {
            IsUnitOnlyButtonEnabled.Value = isActive && !_playerHasUsedUnitOnly;
            IsBuildingOnlyButtonEnabled.Value = isActive && !_playerHasUsedBuildingOnly;
            IsRestrictedButtonEnabled.Value = isActive & !_playerHasUsedRestricted;
        }

        public UpperMenuViewModel()
        {
            PlayerColor.Value = CurrentPlayer.Color;
            ClientSingleton.Instance.serverClient.ListenForResourcesUpdate(OnResourceUpdate);
            ClientSingleton.Instance.serverClient.ListenForGameModeChangeEvent(OnGameModeUpdate);
        }

        private void OnGameModeUpdate(GameModeChangeServerEvent response)
        {
            SetAbilityButtonsState(response.GameModeType == GameModeType.Normal);
            GameModeText.Value = response.GameModeType.ToString();
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
