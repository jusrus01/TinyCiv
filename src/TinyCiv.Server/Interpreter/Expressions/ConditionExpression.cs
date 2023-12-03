using TinyCiv.Server.Core.Services;
using TinyCiv.Server.Interpreter.Tokens;

namespace TinyCiv.Server.Interpreter.Expressions;

public enum Condition
{
    None, // no action, not initialized
    NotUnderAttack, // while no units are attacking this player
    AnyUnitsAlive, // any units still alive
    MoreThanOneUnitLeft, // do some stuff until one unit is left
    AllCurrentUnitsAlive // do some stuff until any unit dies
}

public class ConditionExpression : IExpression
{
    public ConditionExpression(IGameService gameService)
    {
        _gameService = gameService;
    }
    
    private Condition? _parsedCondition;
    private ConditionContext? _context;
    
    private readonly IGameService _gameService;

    public bool Result { get; private set; }
    
    public bool Parse(ITokenStream stream)
    {
        if (stream.Next() is not StringToken token)
        {
            return false;
        }

        var validConditions = Enum.GetValues<Condition>();
        Condition? condition = validConditions.SingleOrDefault(con => string.Equals(con.ToString(), token.Value, StringComparison.InvariantCultureIgnoreCase));
        if (condition == null)
        {
            return false;
        }

        _parsedCondition = condition;

        return true;
    }

    public void Evaluate(Guid playerId)
    {
        ArgumentNullException.ThrowIfNull(_parsedCondition);

        _context ??= new ConditionContext
        {
            Condition = _parsedCondition.Value,
            PlayerId = playerId,
            Result = false,
            UnitCount = 0
        };

        _context = _gameService.EvaluateCondition(_context);

        Result = _context?.Result ?? false;
    }
}