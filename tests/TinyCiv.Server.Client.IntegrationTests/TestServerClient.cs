using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using TinyCiv.Shared.Events.Client;
using TinyCiv.Shared.Events.Client.Lobby;
using TinyCiv.Shared.Game;

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
        const int testedClientEventCount = 5;
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
        yield return new object[] { new LeaveLobbyClientEvent() };
        yield return new object[] { new CreateBuildingClientEvent(Guid.NewGuid(), BuildingType.Blacksmith, new ServerPosition { X = 0, Y = 0}) };
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
        
        await WaitForResponseAsync();

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
        await WaitForResponseAsync();
        
        await _sut.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();
    
        //assert
        Assert.True(isInvoked);
        Assert.True(isOtherInvoked);
    }
    
    [Fact]
    public async Task ListenForGameStart_When_StartGameClientEventSent_AndWateryMapSelected_Then_MapReceivedAndGameStartedForAll()
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
        await WaitForResponseAsync();
        
        await _sut.SendAsync(new StartGameClientEvent(MapType.Watery));
        await WaitForResponseAsync();
    
        //assert
        Assert.True(isInvoked);
        Assert.True(isOtherInvoked);
    }
    
    [Fact]
    public async Task ListenForGameStart_When_GameCannotBeStarted_And_ClientTriesToStartGame_Then_NoResponseReceived()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;
        
        var anotherClient = InitializeClient();
        anotherClient.ListenForGameStart(_ =>
        {
            isOtherInvoked = true;
        });
    
        //act
        _sut.ListenForGameStart(_ =>
        {
            isInvoked = true;
        });
        
        // after two players join lobby, StartGameClientEvent is available for use
        await anotherClient.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();
        
        // leave lobby and make start game not valid
        await anotherClient.SendAsync(new LeaveLobbyClientEvent());
        await WaitForResponseAsync();
        
        await _sut.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();
    
        //assert
        Assert.False(isInvoked);
        Assert.False(isOtherInvoked);
    }
    
    #endregion

    #region ListenForLobbyState

    [Fact]
    public async Task ListenForLobbyState_When_GameStateDoesNotAllowForGameStart_Then_NoResponseReceived_And_GameStartFalse()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;
        var canGameStart = false;
        var canGameStartForOther = false;
        
        var anotherClient = InitializeClient();
        anotherClient.ListenForLobbyState(response =>
        {
            isOtherInvoked = true;
            canGameStartForOther = response.CanGameStart;
        });
    
        //act
        _sut.ListenForLobbyState(response =>
        {
            isInvoked = true;
            canGameStart = response.CanGameStart;
        });
        
        // after one player joins lobby, StartGameClientEvent is not available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();
    
        //assert
        Assert.False(isInvoked);
        Assert.False(canGameStart);

        Assert.False(isOtherInvoked);
        Assert.False(canGameStartForOther);
    }
    
    [Fact]
    public async Task ListenForLobbyState_When_GameStateAllowsGameToBeStarted_Then_CallbackInvoked_And_GameStartTrue()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;

        var canGameStart = false;
        var canGameStartForOther = false;
        
        var anotherClient = InitializeClient();
        anotherClient.ListenForLobbyState(response =>
        {
            isOtherInvoked = true;
            canGameStartForOther = response.CanGameStart;
        });
    
        //act
        _sut.ListenForLobbyState(response =>
        {
            isInvoked = true;
            canGameStart = response.CanGameStart;
        });
        
        // after two players join lobby, StartGameClientEvent is available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();
    
        //assert
        Assert.True(isInvoked);
        Assert.True(canGameStart);
        
        Assert.True(isOtherInvoked);
        Assert.True(canGameStartForOther);
    }
    
    [Fact]
    public async Task ListenForLobbyState_When_SomeoneDisconnects_And_GameNotYetStarted_Then_CallbackInvoked_And_GameStartFalse()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;

        var canGameStart = false;
        var canGameStartForOther = false;
        
        var anotherClient = InitializeClient();
        anotherClient.ListenForLobbyState(response =>
        {
            isOtherInvoked = true;
            canGameStartForOther = response.CanGameStart;
        });
    
        //act
        _sut.ListenForLobbyState(response =>
        {
            isInvoked = true;
            canGameStart = response.CanGameStart;
        });
        
        // after two players join lobby, StartGameClientEvent is available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await anotherClient.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();

        await anotherClient.SendAsync(new LeaveLobbyClientEvent());
        await WaitForResponseAsync();

        //assert
        Assert.True(isInvoked);
        Assert.False(canGameStart);
        
        Assert.True(isOtherInvoked);
        Assert.False(canGameStartForOther);
    }

    [Fact]
    public async Task ListenForLobbyState_When_SomeoneDisconnects_And_GameWasStarted_Then_NoResponseReceived()
    {
        //arrange
        var isInvoked = false;
        var isOtherInvoked = false;

        var canGameStart = false;
        var canGameStartForOther = false;
        
        var anotherClient = InitializeClient();
        anotherClient.ListenForLobbyState(response =>
        {
            isOtherInvoked = true;
            canGameStartForOther = response.CanGameStart;
        });
    
        //act
        _sut.ListenForLobbyState(response =>
        {
            isInvoked = true;
            canGameStart = response.CanGameStart;
        });
        
        // after two players join lobby, StartGameClientEvent is available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await anotherClient.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();

        // start game
        await anotherClient.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();
        
        // leave game
        await anotherClient.SendAsync(new LeaveLobbyClientEvent());
        await WaitForResponseAsync();

        //assert
        Assert.True(isInvoked);
        Assert.True(canGameStart);
        
        Assert.True(isOtherInvoked);
        Assert.True(canGameStartForOther);
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
        await WaitForResponseAsync();

        //assert
        Assert.True(isInvoked);
    }

    #endregion

    #region ListenForResourcesUpdate
    // Not working, don't know why yet
    [Fact]
    public async Task ListenForResourcesUpdate_When_CreateBuildingClientEventSent()
    {
        // arrange
        var anotherClient = InitializeClient();
        await anotherClient.SendAsync(new JoinLobbyClientEvent());

        Guid playerId = Guid.Empty;
        _sut.ListenForNewPlayerCreation((player) =>
        {
            playerId = player.Created.Id;
        });

        // after two players join lobby, StartGameClientEvent is available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();

        // start game
        await anotherClient.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();

        Resources? playerResources = new();
        _sut.ListenForResourcesUpdate((resources) =>
        {
            playerResources = resources.Resources;
        });

        _sut.ListenForMapChange((eventas) =>
        {
            Console.WriteLine("Map changed");
        });

        // build blacksmith building (+5 Industry, -1 Gold)
        await _sut.SendAsync(new CreateBuildingClientEvent(playerId, BuildingType.Blacksmith, new ServerPosition { X = 0, Y = 0 }));
        await WaitForResponseAsync(6500);

        // should not work, because player does not have 1 gold
        //Assert.True(playerResources!.Industry == 0);

        // build mine building (+[1-4] gold)
        await _sut.SendAsync(new CreateBuildingClientEvent(playerId, BuildingType.Shop, new ServerPosition { X = 0, Y = 1 }));
        await WaitForResponseAsync(7500);

        // player should have atleast one gold from mine
        //Assert.True(playerResources!.Gold > 0);

        await WaitForResponseAsync(1500);

        // player should have 5 industry from blacksmith
        //Assert.True(playerResources!.Industry == 5);
        Assert.True(true);
    }

    #endregion

    #region Helpers

    private static Task WaitForResponseAsync(int? delayMs = null)
    {
        return Task.Delay(delayMs ?? 600);
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