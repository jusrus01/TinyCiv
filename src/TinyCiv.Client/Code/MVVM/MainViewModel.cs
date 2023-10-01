using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Client.Code.Core;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code.MVVM.ViewModel;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events.Client;
using System.Windows;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Client.Code.Units;
using System.Threading;
using TinyCiv.Shared.Events.Client.Lobby;

namespace TinyCiv.Client.Code.MVVM
{
    public class MainViewModel
    {
        public GameViewModel GameVM = new GameViewModel();
        public UpperMenuViewModel UpperMenuVM = new UpperMenuViewModel();
        public UnitMenuViewModel UnitMenuVM = new UnitMenuViewModel();

        public ObservableValue<object> Game { get; } = new ObservableValue<object>();
        public ObservableValue<object> UpperMenu { get; } = new ObservableValue<object>();
        public ObservableValue<object> LowerMenu { get; } = new ObservableValue<object>(new LobbyMenuViewModel());

        public ObservableValue<String> IsUnitStatVisible { get; } = new ObservableValue<string>("Hidden");


        public MainViewModel()
        {
            Game.Value = GameVM;
            UpperMenu.Value = UpperMenuVM;


            Thread playerConnectionThread = new Thread(() =>
            {
                ClientSingleton.Instance.WaitForInitialization();
                ClientSingleton.Instance.serverClient.ListenForNewPlayerCreation(OnPlayerJoin);
                ClientSingleton.Instance.serverClient.ListenForGameStart(OnGameStart);
                ClientSingleton.Instance.serverClient.SendAsync(new JoinLobbyClientEvent()).Wait();
            });
            playerConnectionThread.Start();

            
        }

        private void OnPlayerJoin(JoinLobbyServerEvent response)
        {
            if (CurrentPlayer.Instance.player == null)
            {
                CurrentPlayer.Instance.player = response.Created;
                UpperMenuVM.PlayerColor.Value = CurrentPlayer.Color;
            }
            MessageBox.Show($"Player: {CurrentPlayer.Id} has joined the game! They are in the {CurrentPlayer.Color} team!");

            // If the party is full
            if (CurrentPlayer.Instance.player == null)
            {
                MessageBox.Show("The game party is full! Try again later.");
            }
        }

        private void OnGameStart(GameStartServerEvent response)
        {
            LowerMenu.Value = UnitMenuVM;
            GameVM.GameStart(response);
            GameVM.UnitMenuVM = UnitMenuVM;

        }

    }
}
