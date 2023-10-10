using TinyCiv.Server.Core.Services;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Services;

/// <summary>
/// Pattern: Bridge (1 of 2)
/// Reasoning:
/// We can easily switch up implementation that could read the map from the database or
/// retrieve it from some external API, just a matter of adding a new implementation,
/// and no need to change anything in <see cref="MapLoader"/>
/// </summary>
public class LocalFileMapReader : IMapReader
{
    private const string MapsDirectory = "Resources";
    
    private readonly ILogger<LocalFileMapReader> _logger;

    public LocalFileMapReader(ILogger<LocalFileMapReader> logger)
    {
        _logger = logger;
    }
    
    public IList<string> Read(MapType type)
    {
        var mapPath = $"{MapsDirectory}/{type.ToString()}";

        if (!File.Exists(mapPath))
        {
            _logger.LogError("Could not find map '{type}' in path '{path}', directory info: {dir}", type, mapPath, Directory.GetFiles(MapsDirectory));
            throw new InvalidOperationException();
        }
        
        return File.ReadAllLines(mapPath);
    }
}