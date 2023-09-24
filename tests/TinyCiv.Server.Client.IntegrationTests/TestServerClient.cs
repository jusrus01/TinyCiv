using Microsoft.AspNetCore.Mvc.Testing;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Client.IntegrationTests;

public class TestServerClient : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ServerClient _sut;
    
    public TestServerClient(WebApplicationFactory<Program> factory)
    {
        factory.CreateClient();
        
        var hostUrl = factory.Server.BaseAddress.ToString();
        var handler = factory.Server.CreateHandler();
        
        _sut = (ServerClient)ServerClient.Create(
            hostUrl[..^1],
            options =>
            {
                options.HttpMessageHandlerFactory = _ => handler;
            });
    }
    
    [Theory, MemberData(nameof(AvailableEvents_TestData))]
    public async Task SendAsync_When_EventPassedIn_Then_SendsEvents(ClientEvent @event)
    {
        //assert
        await _sut.SendAsync(@event, CancellationToken.None);
    }
    
    public static IEnumerable<object[]> AvailableEvents_TestData()
    {
        yield return new object[] { new JoinLobbyClientEvent() };
        yield return new object[] { new CreateUnitClientEvent(Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7"), 0, 0) };
        yield return new object[] { new MoveUnitClientEvent(Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7"), 0, 0) };
    }
}