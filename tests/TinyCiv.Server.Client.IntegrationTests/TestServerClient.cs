using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using TinyCiv.Shared.Events.Client;

namespace TinyCiv.Server.Client.IntegrationTests;

public class TestServerClient : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    private readonly ServerClient _sut;
    
    public TestServerClient(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _factory.CreateClient();
        
        _sut = InitializeClient();
    }

    [Theory, MemberData(nameof(AvailableEvents_TestData))]
    public async Task SendAsync_When_EventPassedIn_Then_SendsEvents(ClientEvent @event)
    {
        // assert
        await _sut.SendAsync(@event, CancellationToken.None);
    }
    
    public static IEnumerable<object[]> AvailableEvents_TestData()
    {
        const int testedClientEventCount = 3;
        var actualEventCount = Assembly.GetAssembly(typeof(ClientEvent))!.GetTypes()
            .Count(type => typeof(ClientEvent).IsAssignableFrom(type) && !type.IsAbstract);
        if (actualEventCount != testedClientEventCount)
        {
            throw new Exception($"Please add tests for new client event. Tested: {testedClientEventCount}, actual event count: {actualEventCount}");
        }
        
        yield return new object[] { new JoinLobbyClientEvent() };
        yield return new object[] { new CreateUnitClientEvent(Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7"), 0, 0) };
        yield return new object[] { new MoveUnitClientEvent(Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7"), 0, 0) };
    }
    
    [Fact]
    public async Task ListenForGameStart_When_JoinLobbyClientEventSentTwice_Then_MapReceivedAndGameStartedForAll()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;
        
        var anotherClient = InitializeClient(true);
        anotherClient.ListenForGameStart(response =>
        {
            isOtherInvoked = true;
            Assert.NotNull(response.Map);
        });

        //act
        _sut.ListenForGameStart(response =>
        {
            isInvoked = true;
            Assert.NotNull(response.Map);
        });
        
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        
        await WaitForResponse();

        //assert
        Assert.True(isInvoked);
        Assert.True(isOtherInvoked);
    }

    [Fact]
    public async Task ListenForNewPlayerCreation_When_JoinLobbyClientEventSent_Then_ReceiveNewPlayerForCaller()
    {
        //arrange
        var isInvoked = false;
        
        var anotherClient = InitializeClient(true);
        anotherClient.ListenForNewPlayerCreation(_ => throw new Exception("Should not receive message"));

        //act
        _sut.ListenForNewPlayerCreation(response =>
        {
            isInvoked = true;
            
            Assert.NotNull(response.Created);
            Assert.NotEqual(Guid.Empty, response.Created.Id);
        });
        
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponse();

        //assert
        Assert.True(isInvoked);
    }

    private static async Task WaitForResponse()
    {
        await Task.Delay(5000);
    }

    private ServerClient InitializeClient(bool createNew = false)
    {
        var hostUrl = _factory.Server.BaseAddress.ToString();
        var handler = _factory.Server.CreateHandler();
        
        return (ServerClient)ServerClient.Create(
            hostUrl[..^1],
            options =>
            {
                options.HttpMessageHandlerFactory = _ => handler;
            },
            createNew);
    }
}