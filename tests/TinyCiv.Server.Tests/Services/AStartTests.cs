using TinyCiv.Shared.Game;
using TinyCiv.Server.Services;

namespace TinyCiv.Server.Tests.Services;

public class AStarTests
{
    public static IEnumerable<object[]> HeuristicCostEstimateTestData()
    {
        yield return new object[] { new ServerPosition { X = 0, Y = 0 }, new ServerPosition { X = 0, Y = 0 }, 0 };
        yield return new object[] { new ServerPosition { X = 0, Y = 0 }, new ServerPosition { X = 0, Y = 1 }, 1 };
        yield return new object[] { new ServerPosition { X = 0, Y = 0 }, new ServerPosition { X = 1, Y = 0 }, 1 };
        yield return new object[] { new ServerPosition { X = 1, Y = 1 }, new ServerPosition { X = 3, Y = 3 }, 4 };
        yield return new object[] { new ServerPosition { X = 2, Y = 3 }, new ServerPosition { X = 1, Y = 1 }, 3 };
    }


    [Theory]
    [MemberData(nameof(HeuristicCostEstimateTestData))]
    public void HeuristicCostEstimate_ShouldReturnExpectedResult(ServerPosition start, ServerPosition end, int expectedResult)
    {
        // Act
        int result = AStar.HeuristicCostEstimate(start, end);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}