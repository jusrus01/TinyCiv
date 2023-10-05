using System.Threading;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Client.Code.MVVM.ViewModel
{
    public class LobbyMenuViewModel : ObservableObject
    {
        //public ObservableValue<RelayCommand> StartGame { get; } = new ObservableValue<RelayCommand>(new RelayCommand( o => 
        //{
        //    ClientSingleton.Instance.serverClient.SendAsync(new StartGameClientEvent(MapType.Watery));
        //}));

        public RelayCommand StartGame { get; } = new RelayCommand(execute =>
        {
            ClientSingleton.Instance.serverClient.SendAsync(new StartGameClientEvent(MapType.Watery));
        });


        public ObservableValue<bool> IsLobbyReady { get; } = new ObservableValue<bool>(false);

        public LobbyMenuViewModel()
        {
            Thread playerConnectionThread = new Thread(() =>
            {
                ClientSingleton.Instance.WaitForInitialization();
                ClientSingleton.Instance.serverClient.ListenForLobbyState(OnGameStartReady);
            });
            playerConnectionThread.Start();
        }

        private void OnGameStartReady(LobbyStateServerEvent e)
        {
            IsLobbyReady.Value = true;
        }

    }
}
