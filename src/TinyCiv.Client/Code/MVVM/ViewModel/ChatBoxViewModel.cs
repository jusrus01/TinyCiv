using System.Threading.Tasks;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Client.Code.MVVM.ViewModel;

public class ChatBoxViewModel : ObservableObject
{
    public ObservableValue<string> ChatBoxInput { get; } = new();
    
    public RelayCommand SendCommand => new(_ => ProcessInput());

    public void ProcessInput()
    {
        var @event = new InterpretClientEvent(CurrentPlayer.Instance.player.Id, ChatBoxInput.Value);

        Task.Run(() => ClientSingleton.Instance.serverClient.SendAsync(@event));

        ChatBoxInput.Value = null;
    }
}