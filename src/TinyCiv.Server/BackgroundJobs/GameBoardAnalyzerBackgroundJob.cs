using TinyCiv.Server.Core.Publishers;
using TinyCiv.Server.Core.Services;
using TinyCiv.Shared;
using TinyCiv.Shared.Events.Server;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.BackgroundJobs;

public class GameBoardAnalyzerBackgroundJob : BackgroundService
{
    private readonly ILogger<GameBoardAnalyzerBackgroundJob> _logger;
    private readonly IPublisher _publisher;
    private readonly ISessionService _sessionService;
    private readonly IMapService _mapService;

    private const int CheckDelayInMilliseconds = 1000;
    
    private const int ColdStartInMilliseconds = 3000;
    private bool _startedAnalyzing;
    
    public GameBoardAnalyzerBackgroundJob(
        IPublisher publisher,
        ISessionService sessionService,
        IMapService mapService,
        ILogger<GameBoardAnalyzerBackgroundJob> logger)
    {
        _logger = logger;
        _publisher = publisher;
        _sessionService = sessionService;
        _mapService = mapService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(CheckDelayInMilliseconds, stoppingToken);

            if (!_sessionService.IsStarted())
            {
                continue;
            }

            if (!_startedAnalyzing)
            {
                await Task.Delay(ColdStartInMilliseconds, stoppingToken);
                _startedAnalyzing = true;
            }

            var latestScanResult = AnalyzeGameBoard();
            
            var playersToRemove = latestScanResult
                .Where(result => result.Value?.Count == 0)
                .Select(result => result.Key)
                .ToList();
            foreach (var playerId in playersToRemove.Union(_sessionService.GetPlayerIds().Where(i => !latestScanResult.Keys.Contains(i))))
            {
                _sessionService.RemovePlayer(playerId);
                
                await _publisher.NotifyAllAsync(Constants.Server.SendDefeatEventToAll, new DefeatServerEvent(playerId));
                
                _logger.LogWarning("Loser found '{player_id}' and everyone notified", playerId);
            }
            
            if (IsSinglePlayerLeftWithRequiredResources(latestScanResult))
            {
                _sessionService.StopGame();

                var winnerId = latestScanResult.FirstOrDefault(result => result.Value?.Count > 0).Key;
                await _publisher.NotifyAllAsync(Constants.Server.SendVictoryEventToAll, new VictoryServerEvent(winnerId));
                
                _logger.LogWarning("Winner found '{player_id}' and everyone notified", winnerId);
                
                return;
            }
        }
    }

    private static bool IsSinglePlayerLeftWithRequiredResources(Dictionary<Guid, IList<Guid>?> latestScanResult)
    {
        return latestScanResult.Count(scanResult => scanResult.Value?.Count > 0) <= 1;
    }

    private Dictionary<Guid, IList<Guid>?> AnalyzeGameBoard()
    {
        var latestMapState = _mapService.GetMapObjects();

        var latestScanResults = new Dictionary<Guid, IList<Guid>?>();

        foreach (var obj in latestMapState)
        {
            if (obj.Type is GameObjectType.Colonist or GameObjectType.City)
            {
                if (!latestScanResults.ContainsKey(obj.OwnerPlayerId))
                {
                    latestScanResults[obj.OwnerPlayerId] = new List<Guid>();
                }
                
                latestScanResults[obj.OwnerPlayerId]!.Add(obj.Id);
            }
        }

        return latestScanResults;
    }
}