using TinyCiv.Server.Core.Iterators;
using TinyCiv.Server.Core.Services;

namespace TinyCiv.Server.Interpreter.Expressions;

public interface IExpressionStorage
{
    IIterator<IExpression?> GetIterator();
}

public class ExpressionStorage : IExpressionStorage
{
    private readonly IGameService _gameService;
    private readonly List<IExpression> _expressions;

    public ExpressionStorage(IGameService gameService)
    {
        _gameService = gameService;
        _expressions = CreateExpressions();
    }
    
    public IIterator<IExpression?> GetIterator()
    {
        return new ExpressionIterator(_expressions);
    }
    
    private List<IExpression> CreateExpressions()
    {
        var whileExpression = new WhileExpression();
        whileExpression.Add(new ConditionExpression(_gameService));
        whileExpression.Add(new ThenExpression());
        whileExpression.Add(new AttackExpression(_gameService));

        var ifExpression = new IfExpression();
        ifExpression.Add(new ConditionExpression(_gameService));
        ifExpression.Add(new ThenExpression());
        ifExpression.Add(new AttackExpression(_gameService));

        var attackExpression = new AttackExpression(_gameService);
        
        return new List<IExpression>
        {
            whileExpression,
            ifExpression,
            attackExpression
        };
    }
}