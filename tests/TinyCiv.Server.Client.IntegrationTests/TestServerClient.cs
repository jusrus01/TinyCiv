using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using TinyCiv.Shared;
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
        const int testedClientEventCount = 8;
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
        yield return new object[] { new AttackUnitClientEvent(Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7"), Guid.Parse("C71E41FF-24AA-46C9-8CBC-5C2A51702AE7")) };
        yield return new object[] { new CreateBuildingClientEvent(Guid.NewGuid(), BuildingType.Blacksmith, new ServerPosition { X = 0, Y = 0 }) };
        yield return new object[] { new PlaceTownClientEvent(Guid.NewGuid()) };
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

        Resources? playerResources = new();
        _sut.ListenForResourcesUpdate((resources) =>
        {
            playerResources = resources.Resources;
        });

        List<ServerPosition> colonistPositions = new();
        _sut.ListenForGameStart(map =>
        {
            var positions = map.Map.Objects!
                .Where(o => o.Type == GameObjectType.Colonist)
                .Select(o => o.Position)!;

            colonistPositions.AddRange(positions!);
        });

        int changeCount = 0;
        _sut.ListenForMapChange(map =>
        {
            if (changeCount == 3)
            {
                int bankCount = map.Map.Objects!
                    .Where(o => o.Type == GameObjectType.Bank)
                    .Count();
                Assert.Equal(1, bankCount);
            }

            changeCount++;
        });

        // after two players join lobby, StartGameClientEvent is available for use
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();

        // start game
        await anotherClient.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();

        await _sut.SendAsync(new PlaceTownClientEvent(playerId));
        await WaitForResponseAsync();

        // build blacksmith building (+5 Industry, -1 Gold)
        await _sut.SendAsync(new CreateBuildingClientEvent(playerId, BuildingType.Blacksmith, findFreeTile()));
        await WaitForResponseAsync(6500);

        Assert.True(playerResources!.Industry == 105);

        // build bank building (+5 gold)
        await _sut.SendAsync(new CreateBuildingClientEvent(playerId, BuildingType.Bank, findFreeTile()));
        await WaitForResponseAsync(3500);

        // player should have atleast one gold from mine
        Assert.True(playerResources!.Gold == 54);

        // build bank building (+5 gold) (should not be able to build, because doesn't have enough Industry points)
        await _sut.SendAsync(new CreateBuildingClientEvent(playerId, BuildingType.Bank, findFreeTile()));
        await WaitForResponseAsync();

        ServerPosition findFreeTile()
        {
            var random = new Random();
            int x = random.Next(0, Constants.Game.WidthSquareCount);
            int y = random.Next(0, Constants.Game.HeightSquareCount);

            while (colonistPositions.Where(p => p.X == x && p.Y == y).Any())
            {
                x = random.Next(0, Constants.Game.WidthSquareCount);
                y = random.Next(0, Constants.Game.HeightSquareCount);
            }
            
            return new ServerPosition { X = x, Y = y };
        }
    }
    #endregion

    #region ListenForInteractableObjectChanges

    [Fact]
    public async Task ListenForInteractableObjectChanges_WhenUnitDead_Then_ChangedStateReceived()
    {
        //arrange
        var anotherClient = InitializeClient();

        Guid? playerId2 = null;
        anotherClient.ListenForNewPlayerCreation(resp =>
        {
            playerId2 = resp.Created.Id;
        });

        Guid? playerId1 = null;
        _sut.ListenForNewPlayerCreation(resp =>
        {
            playerId1 = resp.Created.Id;
        });

        ServerGameObject? player1GameObject = null;
        _sut.ListenForNewUnitCreation(resp =>
        {
            player1GameObject = resp.CreatedUnit;
        });

        ServerGameObject? player2GameObject = null;
        anotherClient.ListenForNewUnitCreation(resp =>
        {
            player2GameObject = resp.CreatedUnit;
        });

        var isPlayer1Dead = false;
        var isPlayer2Dead = false;
        _sut.ListenForInteractableObjectChanges(resp =>
        {
            if (resp.Health <= 0)
            {
                isPlayer1Dead = true;
            }
        });
        anotherClient.ListenForInteractableObjectChanges(resp =>
        {
            if (resp.Health <= 0)
            {
                isPlayer2Dead = true;
            }
        });

        // after two players join lobby, StartGameClientEvent is available for use
        await anotherClient.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();

        await _sut.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();

        //act
        await _sut.SendAsync(new CreateUnitClientEvent(playerId1!.Value, 1, 4, GameObjectType.Colonist));
        await anotherClient.SendAsync(new CreateUnitClientEvent(playerId2!.Value, 1, 2, GameObjectType.Warrior));
        await WaitForResponseAsync();

        await _sut.SendAsync(new MoveUnitClientEvent(player1GameObject!.Id, 1, 2));
        await WaitForResponseAsync(5000);

        await anotherClient.SendAsync(new AttackUnitClientEvent(player2GameObject!.Id, player1GameObject!.Id));
        await WaitForResponseAsync(5000);

        //assert
        Assert.True(isPlayer1Dead || isPlayer2Dead);
    }

    public static IEnumerable<object[]> ListenForInteractableObjectChanges_TestData()
    {
        yield return new object[] { GameObjectType.Warrior, Constants.Game.Interactable.Warrior.Damage, Constants.Game.Interactable.Warrior.InitialHealth };
        yield return new object[] { GameObjectType.Colonist, Constants.Game.Interactable.Colonist.Damage, Constants.Game.Interactable.Colonist.InitialHealth };
        yield return new object[] { GameObjectType.Cavalry, Constants.Game.Interactable.Cavalry.Damage, Constants.Game.Interactable.Cavalry.InitialHealth };
        yield return new object[] { GameObjectType.Tarran, Constants.Game.Interactable.Tarran.Damage, Constants.Game.Interactable.Tarran.InitialHealth };
        yield return new object[] { GameObjectType.Town, null, null };
        yield return new object[] { GameObjectType.Empty, null, null };
        yield return new object[] { GameObjectType.Mine, null, null };
        yield return new object[] { GameObjectType.StaticMountain, null, null };
    }


    [Theory, MemberData(nameof(ListenForInteractableObjectChanges_TestData))]
    public async Task ListenForInteractableObjectChanges_WhenUnitCreated_Then_ChangedStateReceivedWithInitialValues(GameObjectType type, int? expectedAttackDamage, int? expectedInitialHealth)
    {
        //arrange
        var anotherClient = InitializeClient();

        Guid? playerId = null;
        _sut.ListenForNewPlayerCreation(resp =>
        {
            playerId = resp.Created.Id;
        });


        int? receivedAttackDamage = null;
        int? receivedInitialHealth = null;

        _sut.ListenForInteractableObjectChanges(resp =>
        {
            receivedAttackDamage = resp.AttackDamage;
            receivedInitialHealth = resp.Health;
        });

        // after two players join lobby, StartGameClientEvent is available for use
        await anotherClient.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();

        await _sut.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();

        //act
        await _sut.SendAsync(new CreateUnitClientEvent(playerId!.Value, 1, 1, type));
        await WaitForResponseAsync();

        //assert
        Assert.Equal(expectedAttackDamage, receivedAttackDamage);
        Assert.Equal(expectedInitialHealth, receivedInitialHealth);
    }

    #endregion

    #region TownPlacement
    // Not working, don't know why yet
    [Fact]
    public async Task ListenForMapUpdate_When_CreateTownClientEventSent()
    {
        // arrange
        var anotherClient = InitializeClient();

        Guid? playerId2 = null;
        anotherClient.ListenForNewPlayerCreation(resp =>
        {
            playerId2 = resp.Created.Id;
        });

        Guid? playerId1 = null;
        _sut.ListenForNewPlayerCreation(resp =>
        {
            playerId1 = resp.Created.Id;
        });

        int testIndex = 0;
        _sut.ListenForMapChange((eventas) =>
        {
            if (testIndex == 0)
            {
                int colonistCount = eventas.Map.Objects!
                    .Where(o => o.Type == GameObjectType.Colonist)
                    .Count();
                Assert.Equal(2, colonistCount);
            }

            if (testIndex == 1)
            {
                int townCount = eventas.Map.Objects!
                    .Where(o => o.Type == GameObjectType.Town)
                    .Count();
                Assert.Equal(1, townCount);
            }

            if (testIndex == 2)
            {
                int townCount = eventas.Map.Objects!
                    .Where(o => o.Type == GameObjectType.Town)
                    .Count();
                Assert.Equal(2, townCount);
            }

            testIndex++;
        });

        await anotherClient.SendAsync(new JoinLobbyClientEvent());
        await _sut.SendAsync(new JoinLobbyClientEvent());
        await WaitForResponseAsync();

        await _sut.SendAsync(new StartGameClientEvent());
        await WaitForResponseAsync();

        await _sut.SendAsync(new PlaceTownClientEvent(playerId1!.Value));
        await WaitForResponseAsync();

        await _sut.SendAsync(new PlaceTownClientEvent(playerId1.Value));
        await WaitForResponseAsync();

        await _sut.SendAsync(new PlaceTownClientEvent(playerId2!.Value));
        await WaitForResponseAsync();
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