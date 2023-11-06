using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Tests.Services;

[Collection(nameof(TestInteractableObjectService))]
public class TestInteractableObjectService
{
    private readonly Mock<IMapService> _mapServiceMock;
    
    private readonly InteractableObjectService _sut;
    
    public TestInteractableObjectService()
    {
        _mapServiceMock = new Mock<IMapService>();
        
        _sut = new InteractableObjectService(_mapServiceMock.Object, NullLogger<InteractableObjectService>.Instance);
    }

    public static IEnumerable<object[]> Initialize_TestData()
    {
        yield return new object[] { GameObjectType.City , typeof(InteractableCity) };
        yield return new object[] { GameObjectType.Warrior, typeof(InteractableWarrior) };
        yield return new object[] { GameObjectType.Cavalry, typeof(InteractableCavalry) };
        yield return new object[] { GameObjectType.Tarran, typeof(InteractableTarran) };
    }

    [Theory, MemberData(nameof(Initialize_TestData))]
    public void Initialize_When_InteractableGameObjectPassedIn_Then_Resolves(GameObjectType type, Type expectedType)
    {
        //act
        var result = _sut.Initialize(new ServerGameObject { Type = type });
        
        //assert
        Assert.IsType(expectedType, result);
    }

    public static IEnumerable<object[]> Initialize_Failure_TestData()
    {
        yield return new object[] { GameObjectType.Empty };
        yield return new object[] { GameObjectType.Bank };
        yield return new object[] { GameObjectType.Blacksmith };
        yield return new object[] { GameObjectType.Farm };
        yield return new object[] { GameObjectType.Mine };
        yield return new object[] { GameObjectType.Port };
        yield return new object[] { GameObjectType.Shop };
        yield return new object[] { GameObjectType.StaticWater };
        yield return new object[] { GameObjectType.StaticMountain };
    }
    
    [Theory, MemberData(nameof(Initialize_Failure_TestData))]
    public void Initialize_When_IncompatibleObjectPassedIn_Then_Throws(GameObjectType type)
    {
        //assert
        Assert.Throws<InvalidOperationException>(() => _sut.Initialize(new ServerGameObject { Type = type }));
    }
    
    [Fact]
    public void Initialize_When_ObjectAlreadyAdded_Then_SameReferenceReturned()
    {
        //arrange
        var gameObject = new ServerGameObject { Type = GameObjectType.Tarran };
        
        //act
        var result = _sut.Initialize(gameObject);
        var result2 = _sut.Initialize(gameObject);
        
        //assert
        Assert.Equal(result, result2);
    }
    
    public static IEnumerable<object[]> GetInfo_TestData()
    {
        yield return new object[] { GameObjectType.City , typeof(InteractableCity) };
        yield return new object[] { GameObjectType.Warrior, typeof(InteractableWarrior) };
        yield return new object[] { GameObjectType.Cavalry, typeof(InteractableCavalry) };
        yield return new object[] { GameObjectType.Tarran, typeof(InteractableTarran) };
    }

    [Theory, MemberData(nameof(GetInfo_TestData))]
    public void GetInfo_When_InteractableGameObjectTypePassedIn_Then_Resolves(GameObjectType type, Type expectedType)
    {
        //act
        var result = _sut.GetInfo(type);
        
        //assert
        Assert.IsType(expectedType, result);
    }

    public static IEnumerable<object[]> GetInfo_Failure_TestData()
    {
        yield return new object[] { GameObjectType.Empty };
        yield return new object[] { GameObjectType.Bank };
        yield return new object[] { GameObjectType.Blacksmith };
        yield return new object[] { GameObjectType.Farm };
        yield return new object[] { GameObjectType.Mine };
        yield return new object[] { GameObjectType.Port };
        yield return new object[] { GameObjectType.Shop };
        yield return new object[] { GameObjectType.StaticWater };
        yield return new object[] { GameObjectType.StaticMountain };
    }
    
    [Theory, MemberData(nameof(GetInfo_Failure_TestData))]
    public void GetInfo_When_IncompatibleObjectTypePassedIn_Then_ReturnsNull(GameObjectType type)
    {
        //act
        var result = _sut.GetInfo(type);
        
        //assert
        Assert.Null(result);
    }

    [Fact]
    public void Get_When_InteractableExists_Then_ReturnsInteractable()
    {
        //arrange
        var id = Guid.Parse("462C50AD-3FCF-488F-9976-698F12D1AEDE");
        var gameObject = new ServerGameObject { Id = id, Type = GameObjectType.Warrior };
        _sut.Initialize(gameObject);

        //act
        var result = _sut.Get(id);

        //assert
        Assert.IsType<InteractableWarrior>(result);
    }
    
    [Fact]
    public void Get_When_InteractableIsNotFound_Then_ReturnsNull()
    {
        //arrange
        var id = Guid.Parse("462C50AD-3FCF-488F-9976-698F12D1AEDE");

        //act
        var result = _sut.Get(id);

        //assert
        Assert.Null(result);
    }

    [Fact]
    public void RegisterClone_When_InteractablePassedIn_Then_Registers()
    {
        //act
        _sut.RegisterClone(new InteractableCavalry());
        
        //assert
        Assert.NotEmpty(_sut.FlushClones());
    }

    [Fact]
    public void FlushClones_When_ClonesExists_Then_Returns()
    {
        //arrange
        _sut.RegisterClone(new InteractableCavalry());
        
        //act
        var result = _sut.FlushClones();
        
        //assert
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public void FlushClones_When_Empty_Then_ReturnsEmpty()
    {
        //act
        var result = _sut.FlushClones();
        
        //assert
        Assert.Empty(result);
    }

    [Fact]
    public void Remove_When_Called_Then_Removes()
    {
        //arrange
        var id = Guid.Parse("C5FAA39E-90F2-4B15-9E94-72E3F32CB992");
        var gameObject = new ServerGameObject { Id = id, Type = GameObjectType.Tarran };
        _sut.Initialize(gameObject);
        
        //act
        _sut.Remove(id);
        
        //assert
        Assert.Null(_sut.Get(id));
    }

    [Fact]
    public async Task TransformClonesToGameObjectsAsync_When_Called_Then_CallbacksInvoked()
    {
        //arrange
        _mapServiceMock
            .Setup(i => i.GetMap())
            .Returns(new Map());

        _mapServiceMock
            .Setup(i => i.TryFindClosestAvailablePosition(It.IsAny<ServerPosition>()))
            .Returns(new ServerPosition());
        
        _mapServiceMock
            .Setup(i => i.CreateUnit(It.IsAny<Guid>(), It.IsAny<ServerPosition>(), It.IsAny<GameObjectType>()))
            .Returns(new ServerGameObject());

        _mapServiceMock
            .Setup(i => i.GetUnit(It.IsAny<Guid>()))
            .Returns(new ServerGameObject());

        var mapChangeNotifierInvoked = false;
        var newUnitNotifierInvoked = false;
        var attackStateNotifierInvoked = false;
        
        //act
        await _sut.TransformClonesToGameObjectsAsync(
            new List<IInteractableObject> { new InteractableCity() },
            _ =>
            {
                mapChangeNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                attackStateNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                newUnitNotifierInvoked = true;
                return Task.CompletedTask;
            });
        
        //assert
        Assert.True(mapChangeNotifierInvoked);
        Assert.True(newUnitNotifierInvoked);
        Assert.True(attackStateNotifierInvoked);
    }
    
    [Fact]
    public async Task TransformClonesToGameObjectsAsync_When_UnitNotFoundReferenceId_Then_NewUnitNotifierNotInvoked()
    {
        //arrange
        _mapServiceMock
            .Setup(i => i.GetMap())
            .Returns(new Map());

        _mapServiceMock
            .Setup(i => i.TryFindClosestAvailablePosition(It.IsAny<ServerPosition>()))
            .Returns(new ServerPosition());
        
        _mapServiceMock
            .Setup(i => i.CreateUnit(It.IsAny<Guid>(), It.IsAny<ServerPosition>(), It.IsAny<GameObjectType>()))
            .Returns(new ServerGameObject());

        _mapServiceMock
            .Setup(i => i.GetUnit(It.IsAny<Guid>()))
            .Returns(null as ServerGameObject);

        var mapChangeNotifierInvoked = false;
        var newUnitNotifierInvoked = false;
        var attackStateNotifierInvoked = false;
        
        //act
        await _sut.TransformClonesToGameObjectsAsync(
            new List<IInteractableObject> { new InteractableCity() },
            _ =>
            {
                mapChangeNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                attackStateNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                newUnitNotifierInvoked = true;
                return Task.CompletedTask;
            });
        
        //assert
        Assert.False(newUnitNotifierInvoked);
        Assert.True(mapChangeNotifierInvoked);
        Assert.False(attackStateNotifierInvoked);
    }
    
    [Fact]
    public async Task TransformClonesToGameObjectsAsync_When_ValidPositionNotFoundForClone_Then_NewUnitNotifierNotInvoked()
    {
        //arrange
        _mapServiceMock
            .Setup(i => i.GetMap())
            .Returns(new Map());

        _mapServiceMock
            .Setup(i => i.TryFindClosestAvailablePosition(It.IsAny<ServerPosition>()))
            .Returns(null as ServerPosition);
        
        _mapServiceMock
            .Setup(i => i.CreateUnit(It.IsAny<Guid>(), It.IsAny<ServerPosition>(), It.IsAny<GameObjectType>()))
            .Returns(new ServerGameObject());

        _mapServiceMock
            .Setup(i => i.GetUnit(It.IsAny<Guid>()))
            .Returns(new ServerGameObject());

        var mapChangeNotifierInvoked = false;
        var newUnitNotifierInvoked = false;
        var attackStateNotifierInvoked = false;
        
        //act
        await _sut.TransformClonesToGameObjectsAsync(
            new List<IInteractableObject> { new InteractableCity() },
            _ =>
            {
                mapChangeNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                attackStateNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                newUnitNotifierInvoked = true;
                return Task.CompletedTask;
            });
        
        //assert
        Assert.False(newUnitNotifierInvoked);
        Assert.True(mapChangeNotifierInvoked);
        Assert.False(attackStateNotifierInvoked);
    }
    
    [Fact]
    public async Task TransformClonesToGameObjectsAsync_When_FailedToCreateNewClone_Then_NewUnitNotifierNotInvoked()
    {
        //arrange
        _mapServiceMock
            .Setup(i => i.GetMap())
            .Returns(new Map());

        _mapServiceMock
            .Setup(i => i.TryFindClosestAvailablePosition(It.IsAny<ServerPosition>()))
            .Returns(new ServerPosition());
        
        _mapServiceMock
            .Setup(i => i.CreateUnit(It.IsAny<Guid>(), It.IsAny<ServerPosition>(), It.IsAny<GameObjectType>()))
            .Returns(null as ServerGameObject);

        _mapServiceMock
            .Setup(i => i.GetUnit(It.IsAny<Guid>()))
            .Returns(new ServerGameObject());

        var mapChangeNotifierInvoked = false;
        var newUnitNotifierInvoked = false;
        var attackStateNotifierInvoked = false;
        
        //act
        await _sut.TransformClonesToGameObjectsAsync(
            new List<IInteractableObject> { new InteractableCity() },
            _ =>
            {
                mapChangeNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                attackStateNotifierInvoked = true;
                return Task.CompletedTask;
            },
            _ =>
            {
                newUnitNotifierInvoked = true;
                return Task.CompletedTask;
            });
        
        //assert
        Assert.False(newUnitNotifierInvoked);
        Assert.True(mapChangeNotifierInvoked);
        Assert.False(attackStateNotifierInvoked);
    }
}