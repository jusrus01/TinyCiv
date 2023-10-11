using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

/// <summary>
/// Pattern: Bridge (1 of 2)
/// Reasoning:
/// We can easily switch up implementation that could read the map from the database or
/// retrieve it from some external API, just a matter of adding a new implementation,
/// and no need to change anything in <see cref="MapLoader"/>
/// </summary>
public class MapLoader : IMapLoader
{
    private readonly IMapReader _mapReader;
    private readonly ILogger<MapLoader> _logger;

    public MapLoader(IMapReader mapReader, ILogger<MapLoader> logger)
    {
        _logger = logger;
        _mapReader = mapReader;
    }

    public List<ServerGameObject> Load(MapType type)
    {
        var mapContent = _mapReader.Read(type);
        
        EnsureValid(mapContent);
        return GenerateObjects(mapContent);
    }

    private List<ServerGameObject> GenerateObjects(IList<string> mapContent)
    {
        var objects = new List<ServerGameObject>();
        for (var y = 0; y < mapContent.Count; y++)
        {
            var line = mapContent[y];

            for (var x = 0; x < line.Length; x++)
            {
                var obj = new ServerGameObject
                {
                    Id = Guid.NewGuid(),
                    Position = new ServerPosition
                    {
                        X = x,
                        Y = y
                    },
                    Type = ResolveType(line[x])
                };
                
                objects.Add(obj);
            }
        }

        return objects;
    }

    private GameObjectType ResolveType(char c)
    {
        return c switch
        {
            '-' => GameObjectType.Empty,
            'w' => GameObjectType.StaticWater,
            'm' => GameObjectType.StaticMountain,
            _ => throw new InvalidOperationException()
        };
    }

    private void EnsureValid(IList<string> mapContent)
    {
        if (mapContent.Count != Constants.Game.HeightSquareCount)
        {
            _logger.LogError("Map '{map}' has '{rows}', but should have '{expected}'", mapContent, mapContent.Count, Constants.Game.HeightSquareCount);
            throw new InvalidOperationException();
        }

        foreach (var line in mapContent)
        {
            if (line.Length != Constants.Game.WidthSquareCount)
            {
                _logger.LogError("Map '{map}' has some row with greater than '{cols}' square count", mapContent, Constants.Game.WidthSquareCount);
                throw new InvalidOperationException();
            }

            foreach (var gameObjectType in line)
            {
                try
                {
                    _ = ResolveType(gameObjectType);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Map contains unsupported game object '{type}', update {method_name} method with new mapping", gameObjectType, nameof(ResolveType));
                    throw;
                }
            }
        }
    }
}