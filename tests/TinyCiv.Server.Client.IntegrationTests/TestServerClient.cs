using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Client.Lobby;

namespace TinyCiv.Server.Client.IntegrationTests;

public class TestServerClient : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    
    private readonly ServerClient _sut;
    
    public TestServerClient()
    {
        _factory = new WebApplicationFactory<Program>();
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
        const int testedClientEventCount = 4;
        var actualEventCount = Assembly.GetAssembly(typeof(ClientEvent))!.GetTypes()
            .Count(type => typeof(ClientEvent).IsAssignableFrom(type) && !type.IsAbstract);
        if (actualEventCount != testedClientEventCount)
        {
            throw new Exception($"Please add tests for new client event. Tested: {testedClientEventCount}, actual event count: {actualEventCount}");
        }
        
        yield return new object[] { new JoinLobbyClientEvent() };
        yield return new object[] { new CreateUnitClientEvent(Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7"), 0, 0) };
        yield return new object[] { new MoveUnitClientEvent(Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7"), 0, 0) };
        yield return new object[] { new StartGameClientEvent() };
    }

    #region ListenForGameStart

    [Fact]
    public async Task ListenForGameStart_When_StartGameClientEventSent_And_GameStateIsNotReadyToStart_Then_NoResponseReceived()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;
        
        var anotherClient = InitializeClient();
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
        await _sut.SendAsync(new StartGameClientEvent());
        
        await WaitForResponse();

        //assert
        Assert.False(isInvoked);
        Assert.False(isOtherInvoked);
    }
    
    [Fact]
    public async Task ListenForGameStart_When_StartGameClientEventSent_Then_MapReceivedAndGameStartedForAll()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;
        
        var anotherClient = InitializeClient();
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
        
        // after two players join lobby, StartGameClientEvent is available for use
        await anotherClient.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponse();
        
        await _sut.SendAsync(new StartGameClientEvent());
        await WaitForResponse();
    
        //assert
        Assert.True(isInvoked);
        Assert.True(isOtherInvoked);
    }
    
    #endregion

    #region ListenForGameStartReady

    [Fact]
    public async Task ListenForGameStartReady_When_GameStateDoesNotAllowForGameStart_Then_NoResponseReceived()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;
        
        var anotherClient = InitializeClient();
        anotherClient.ListenForGameStartReady(_ =>
        {
            isOtherInvoked = true;
        });
    
        //act
        _sut.ListenForGameStartReady(_ =>
        {
            isInvoked = true;
        });
        
        // after one player joins lobby, StartGameClientEvent is not available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponse();
    
        //assert
        Assert.False(isInvoked);
        Assert.False(isOtherInvoked);
    }
    
    [Fact]
    public async Task ListenForGameStartReady_When_GameStateAllowsGameToBeStarted_Then_CallbackInvoked()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;
        
        var anotherClient = InitializeClient();
        anotherClient.ListenForGameStartReady(_ =>
        {
            isOtherInvoked = true;
        });
    
        //act
        _sut.ListenForGameStartReady(_ =>
        {
            isInvoked = true;
        });
        
        // after two players join lobby, StartGameClientEvent is available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponse();
    
        //assert
        Assert.True(isInvoked);
        Assert.True(isOtherInvoked);
    }

    #endregion

    #region ListenForNewPlayerCreation
    
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
    
    #endregion

    #region Helpers

    private static async Task WaitForResponse()
    {
        await Task.Delay(500);
    }

    private ServerClient InitializeClient(bool createNew = true)
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

    #endregion
    
    public void Dispose()
    {
        _factory.Dispose();
    }
}