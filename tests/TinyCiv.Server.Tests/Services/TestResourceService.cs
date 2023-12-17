using Moq;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;
using TinyCiv.Server.Services;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Dtos.Buildings;
using TinyCiv.Server.Core.Game.Buildings;
using Microsoft.Extensions.Logging.Abstractions;
using TinyCiv.Server.Core.Game.InteractableObjects;
using TinyCiv.Server.Core.Publishers;

namespace TinyCiv.Server.Tests.Services;

[Collection(nameof(TestResourceService))]
public class TestResourceService
{
    private readonly Mock<IMapService> _mapServiceMock;

    private readonly GameService _gameService;
    private readonly ResourceService _sut;
    private readonly Guid _playerId;

    public TestResourceService()
    {
        _mapServiceMock = new Mock<IMapService>();
        _sut = new ResourceService();

        _gameService = new GameService(
            new Mock<ISessionService>().Object,
            new Mock<IConnectionIdAccessor>().Object,
            _sut,
            _mapServiceMock.Object,
            new Mock<IInteractableObjectService>().Object,
            new Mock<ICombatService>().Object,
            new Mock<GameStateService>().Object,
            new Mock<IPublisher>().Object,
            NullLogger<GameService>.Instance);

        _playerId = Guid.NewGuid();
    }

    [Fact]
    public void InitializeResources_Then_Returns()
    {
        _sut.InitializeResources(_playerId);

        var resources = _sut.GetResources(_playerId);

        Assert.Equal(Constants.Game.StartingFood, resources.Food);
        Assert.Equal(Constants.Game.StartingIndustry, resources.Industry);
        Assert.Equal(Constants.Game.StartingGold, resources.Gold);
    }

    public static IEnumerable<object[]> AddResources_TestData()
    {
        yield return new object[] {
            new List<(ResourceType, int)> { (ResourceType.Industry, 5), (ResourceType.Food, 15), (ResourceType.Gold, -50) }
        };

        yield return new object[] {
            new List<(ResourceType, int)> { (ResourceType.Industry, -999999), (ResourceType.Food, -999999), (ResourceType.Gold, -999999) }
        };

        yield return new object[] {
            new List<(ResourceType, int)> { (ResourceType.Industry, 999999), (ResourceType.Food, 999999), (ResourceType.Gold, 999999) }
        };
    }

    [Theory, MemberData(nameof(AddResources_TestData))]
    public void AddResources_Then_Returns(List<(ResourceType, int)> resourcesToAdd)
    {
        _sut.InitializeResources(_playerId);

        resourcesToAdd.ForEach(p => _sut.AddResources(_playerId, p.Item1, p.Item2));

        int totalGoldToAdd = resourcesToAdd.Where(r => r.Item1 == ResourceType.Gold).Sum(r => r.Item2);
        int totalFoodToAdd = resourcesToAdd.Where(r => r.Item1 == ResourceType.Food).Sum(r => r.Item2);
        int totalIndustryToAdd = resourcesToAdd.Where(r => r.Item1 == ResourceType.Industry).Sum(r => r.Item2);
        var resources = _sut.GetResources(_playerId);

        Assert.Equal(Constants.Game.StartingFood + totalFoodToAdd, resources.Food);
        Assert.Equal(Constants.Game.StartingGold + totalGoldToAdd, resources.Gold);
        Assert.Equal(Constants.Game.StartingIndustry + totalIndustryToAdd, resources.Industry);
    }

    public static IEnumerable<object[]> AddBuilding_TestData()
    {
        yield return new object[] { BuildingType.Bank, Constants.Game.BankPrice };
        yield return new object[] { BuildingType.Farm, Constants.Game.FarmPrice };
        yield return new object[] { BuildingType.Mine, Constants.Game.MinePrice };
        yield return new object[] { BuildingType.Port, Constants.Game.PortPrice };
        yield return new object[] { BuildingType.Shop, Constants.Game.ShopPrice };
        yield return new object[] { BuildingType.Blacksmith, Constants.Game.BlacksmithPrice };
    }

    [Theory, MemberData(nameof(AddBuilding_TestData))]
    public void AddBuilding_RemovesResources(BuildingType buildingType, int price)
    {
        _sut.InitializeResources(_playerId);

        _mapServiceMock
            .Setup(m => m.IsTownOwner(It.IsAny<Guid>()))
            .Returns(true);

        _mapServiceMock
            .Setup(m => m.CreateBuilding(It.IsAny<Guid>(), It.IsAny<ServerPosition>(), It.IsAny<IBuilding>()))
            .Returns(new ServerGameObject());

        var createBuildingRequest = new CreateBuildingRequest(_playerId, buildingType, new ServerPosition { X = 0, Y = 0 }, (r) => { });
        var createBuildingResponse = _gameService.CreateBuilding(createBuildingRequest);

        if (createBuildingResponse == null)
        {
            Assert.Fail($"{nameof(createBuildingResponse)} is null");
        }

        Assert.Equal(Constants.Game.StartingIndustry - price, createBuildingResponse.Resources.Industry);
    }

    [Fact]
    public void BuyInteractable_ResourcesNotInitialized_ReturnsNull()
    {
        var response = _sut.BuyInteractable(_playerId, new InteractableWarrior());

        Assert.Null(response);
    }

    [Fact]
    public void CancelInteractablePayment_ResourcesNotInitialized_DoesntThrowException()
    {
        try
        {
            _sut.CancelInteractablePayment(_playerId, new InteractableWarrior());
        }
        catch (Exception)
        {
            Assert.Fail("CancelInteractablePayment failed - Resources are not initialized");
        }
    }

    [Fact]
    public void CancelInteractablePayment_IntercatableIsNull_DoesntThrowException()
    {
        _sut.InitializeResources(_playerId);

        try
        {
            _sut.CancelInteractablePayment(_playerId, null);
        }
        catch (Exception)
        {
            Assert.Fail("CancelInteractablePayment failed - Interactable is null");
        }
    }
}
