using TinyCiv.Server.Core.Iterators;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Game;

public class PlayerIterator : IIterator<Player?>
{
    private readonly HashSet<Player> _players;

    public PlayerIterator(HashSet<Player> players)
    {
        _players = players;
    }
    
    public Player? Next()
    {
        if (!HasNext())
        {
            return null;
        }
        
        var player = _players.First();
        if (!_players.Remove(player))
        {
            throw new InvalidDataException();
        }

        return player;
    }

    public bool HasNext()
    {
        return _players.Any();
    }
}