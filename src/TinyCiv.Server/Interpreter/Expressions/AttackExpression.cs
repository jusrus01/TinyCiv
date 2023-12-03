using TinyCiv.Server.Core.Extensions;
using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Interpreter.Tokens;
using TinyCiv.Shared.Game;

namespace TinyCiv.Server.Interpreter.Expressions;

public class AttackExpression : IExpression
{
    private readonly IGameService _gameService;

    private const string ExpressionActivationKey = "atk";
    
    private GameObjectType? _playerUnits;
    private GameObjectType? _enemyUnits;
    private TeamColor? _enemyColor;
    
    public AttackExpression(IGameService gameService)
    {
        _gameService = gameService;
    }
    
    public bool Parse(ITokenStream stream)
    {
        if (!TryParseToken<GameObjectType>(stream, out var playerUnitType, type => type.IsInteractable()))
        {
            return false;
        }
        _playerUnits = playerUnitType;

        if (!TryParseActivationKey(stream))
        {
            return false;
        }

        if (!TryParseToken<GameObjectType>(stream, out var enemyUnitType, type => type.IsInteractable()))
        {
            return false;
        }
        
        _enemyUnits = enemyUnitType;

        if (!TryParseToken<TeamColor>(stream, out var enemyColor))
        {
            return false;
        }
        _enemyColor = enemyColor;

        return true;
    }

    public void Evaluate(Guid playerId)
    {
        if (_playerUnits == null || _enemyColor == null || _enemyUnits == null)
        {
            throw new Exception();
        }
        
        _gameService.PerformMassAttackOnFirstEnemyUnit(playerId, _playerUnits.Value, _enemyUnits.Value, _enemyColor.Value);
    }
    
    private static bool TryParseActivationKey(ITokenStream stream)
    {
        var activationToken = stream.Next() as StringToken;
        return activationToken != null && activationToken.Value.ToLowerInvariant() == ExpressionActivationKey;
    }

    private static bool TryParseToken<TEnum>(ITokenStream stream, out TEnum result, Func<TEnum, bool>? predicate = null) where TEnum : struct, Enum
    {
        result = default;
        var token = stream.Next() as StringToken;
        if (token == null)
        {
            return false;
        }

        var values = Enum.GetValues<TEnum>();
        
        TEnum? parsedEnum = values.SingleOrDefault(type =>
            string.Equals(type.ToString(), token.Value, StringComparison.InvariantCultureIgnoreCase) &&
            (predicate == null || predicate(type)));
        if (parsedEnum == null)
        {
            return false;
        }

        result = parsedEnum.Value;
        return true;
    }
}