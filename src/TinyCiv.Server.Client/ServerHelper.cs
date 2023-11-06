using System.Text.Json;
using TinyCiv.Shared.Events.Server;

namespace TinyCiv.Server.Client;

internal static class ServerHelper
{
    public static ServerEvent ResolveEvent(string content, string type)
    {
        if (type == nameof(JoinLobbyServerEvent))
        {
            return JsonSerializer.Deserialize<JoinLobbyServerEvent>(content)!;
        }

        if (type == nameof(GameStartServerEvent))
        {
            return JsonSerializer.Deserialize<GameStartServerEvent>(content)!;
        }

        if (type == nameof(MapChangeServerEvent))
        {
            return JsonSerializer.Deserialize<MapChangeServerEvent>(content)!;
        }

        if (type == nameof(CreateUnitServerEvent))
        {
            return JsonSerializer.Deserialize<CreateUnitServerEvent>(content)!;
        }

        if (type == nameof(UnitStatusUpdateServerEvent))
        {
            return JsonSerializer.Deserialize<UnitStatusUpdateServerEvent>(content)!;
        }

        if (type == nameof(LobbyStateServerEvent))
        {
            return JsonSerializer.Deserialize<LobbyStateServerEvent>(content)!;
        }

        if (type == nameof(ResourcesUpdateServerEvent))
        {
            return JsonSerializer.Deserialize<ResourcesUpdateServerEvent>(content)!;
        }

        if (type == nameof(InteractableObjectServerEvent))
        {
            return JsonSerializer.Deserialize<InteractableObjectServerEvent>(content)!;
        }

        if (type == nameof(VictoryServerEvent))
        {
            return JsonSerializer.Deserialize<VictoryServerEvent>(content)!;
        }

        if (type == nameof(DefeatServerEvent))
        {
            return JsonSerializer.Deserialize<DefeatServerEvent>(content)!;
        }

        throw new NotSupportedException();
    }
}
