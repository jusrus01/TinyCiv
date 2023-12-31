﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCiv.Shared.Game;
using TinyCiv.Client.Code.MVVM.ViewModel;
using TinyCiv.Server.Client;
using TinyCiv.Shared.Events.Client;
using System.Windows;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Client.Code.Units;
using System.Threading;
using System.ComponentModel;
using TinyCiv.Shared.Events.Client.Lobby;
using TinyCiv.Shared;
using TinyCiv.Client.Code.MVVM.View;

namespace TinyCiv.Client.Code.MVVM
{
    public class MainViewModel
    {
        public GameViewModel GameVM = new GameViewModel();

        public ObservableValue<object> Game { get; } = new ObservableValue<object>();
        public ObservableValue<object> UpperMenu { get; } = new ObservableValue<object>();
        public ObservableValue<object> LowerMenu { get; } = new ObservableValue<object>(new LobbyMenuViewModel());
        public ObservableValue<ExecutionQueueViewModel> ExecutionMenu { get; } = new ObservableValue<ExecutionQueueViewModel>();
        public ObservableValue<ChatBoxViewModel> ChatBoxMenu { get; } = new();

        public MainViewModel()
        {
            Game.Value = GameVM;
            HUDManager.Instance.mainVM = this;

            DependencyObject dep = new DependencyObject();
            if (!DesignerProperties.GetIsInDesignMode(dep))
            {
                Thread playerConnectionThread = new Thread(() =>
                {
                    ClientSingleton.Instance.WaitForInitialization();
                    ClientSingleton.Instance.serverClient.ListenForNewPlayerCreation(OnPlayerJoin);
                    ClientSingleton.Instance.serverClient.ListenForGameStart(OnGameStart);
                    ClientSingleton.Instance.serverClient.ListenForVictoryEvent(OnVictory);
                    ClientSingleton.Instance.serverClient.ListenForDefeatEvent(OnDefeat);
                    ClientSingleton.Instance.serverClient.SendAsync(new JoinLobbyClientEvent()).Wait();
                });
                playerConnectionThread.Start();
            }
        }

        private void OnPlayerJoin(JoinLobbyServerEvent response)
        {
            if (CurrentPlayer.Instance.player == null)
            {
                CurrentPlayer.Instance.player = response.Created;
                CurrentPlayer.Instance.Resources = new Resources{ Industry = Constants.Game.StartingIndustry, Food = Constants.Game.StartingFood, Gold = Constants.Game.StartingGold };                
                HUDManager.Instance.DisplayUpperMenu();
                HUDManager.Instance.DisplayExecutionQueue();
            }

            // If the party is full
            if (CurrentPlayer.Instance.player == null)
            {
            }
        }

        private void OnGameStart(GameStartServerEvent response)
        {
            GameVM.GameStart(response);
            HUDManager.Instance.HideLowerMenu();       
            HUDManager.Instance.DisplayChatBox();
        }

        private void OnVictory(VictoryServerEvent response)
        {
            if (response.PlayerId == CurrentPlayer.Id)
                HUDManager.Instance.FinishGameVictory();
        }

        private void OnDefeat(DefeatServerEvent response)
        {
            if (response.PlayerId == CurrentPlayer.Id)
                HUDManager.Instance.FinishGameDefeat();
        }
    }
}
