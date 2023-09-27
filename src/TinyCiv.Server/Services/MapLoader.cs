using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

public class MapLoader : IMapLoader
{
    private const string MapsDirectory = "Resources";

    private readonly ILogger<MapLoader> _logger;

    public MapLoader(ILogger<MapLoader> logger)
    {
        _logger = logger;
    }

    public List<ServerGameObject> Load(MapType type)
    {
        var mapPath = $"{MapsDirectory}/{type.ToString()}";

        if (!File.Exists(mapPath))
        {
            _logger.LogError("Could not find map '{type}' in path '{path}', directory info: {dir}", type, mapPath, Directory.GetFiles(MapsDirectory));
            throw new InvalidOperationException();
        }
        
        var mapContent = File.ReadAllLines(mapPath);
        EnsureValid(mapContent);
        return GenerateObjects(mapContent);;
    }

    private List<ServerGameObject> GenerateObjects(string[] mapContent)
    {
        var objects = new List<ServerGameObject>();
        for (var y = 0; y < mapContent.Length; y++)
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

    private void EnsureValid(IReadOnlyCollection<string> mapContent)
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