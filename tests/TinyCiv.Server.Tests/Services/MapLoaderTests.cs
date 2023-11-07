using Microsoft.Extensions.Logging;
using Moq;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Tests.Services;

public class MapLoaderTests
{
    [Fact]
    public void Load_ValidMap_ReturnsServerGameObjects()
    {
        // Arrange
        var mapContent = new List<string>
        {
            "-wm-----------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
        };

        var mapReaderMock = new Mock<IMapReader>();
        mapReaderMock.Setup(reader => reader.Read(It.IsAny<MapType>())).Returns(mapContent);

        var loggerMock = new Mock<ILogger<MapLoader>>();

        var mapLoader = new MapLoader(mapReaderMock.Object, loggerMock.Object);

        // Act
        var result = mapLoader.Load(MapType.Watery);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(GameObjectType.Empty, result[0].Type);
        Assert.Equal(GameObjectType.StaticWater, result[1].Type);
        Assert.Equal(GameObjectType.StaticMountain, result[2].Type);
    }

    [Fact]
    public void Load_InvalidMap_LogsError()
    {
        // Arrange
        var invalidMapContent = new List<string>
        {
            "-wm-----------------",
            "-----xx-----x-------", // Invalid character 'x'
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
            "--------------------",
        };

        var mapReaderMock = new Mock<IMapReader>();
        mapReaderMock.Setup(reader => reader.Read(It.IsAny<MapType>())).Returns(invalidMapContent);

        var loggerMock = new Mock<ILogger<MapLoader>>();

        var mapLoader = new MapLoader(mapReaderMock.Object, loggerMock.Object);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => mapLoader.Load(MapType.Watery));
        
        loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Map contains unsupported game object")),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
    }
}