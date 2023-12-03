using TinyCiv.Server.Interpreter.Tokens;

namespace TinyCiv.Server.Interpreter.Expressions;

public class WhileExpression : IExpression
{
    private const string ExpressionActivationKey = "while";
    private const int EvaluateAfterMilliseconds = 3000;

    private readonly IList<IExpression> _expressions = new List<IExpression>();

    public void Add(IExpression expression)
    {
        if (!_expressions.Any() && expression is not ConditionExpression)
        {
            throw new ArgumentException("First expression must be a condition");
        }
        
        _expressions.Add(expression);
    }
    
    public bool Parse(ITokenStream stream)
    {
        if (stream.Next() is not StringToken token)
        {
            return false;
        }

        if (token.Value.ToLowerInvariant() != ExpressionActivationKey)
        {
            return false;
        }

        foreach (var expression in _expressions)
        {
            if (!expression.Parse(stream))
            {
                return false;
            }
        }

        return true;
    }

    public void Evaluate(Guid playerId)
    {
        if (_expressions.First() is not ConditionExpression condExpr)
        {
            throw new Exception();
        }
        _expressions.RemoveAt(0);

        condExpr.Evaluate(playerId);
        while (condExpr.Result)
        {
            foreach (var expr in _expressions)
            {
                expr.Evaluate(playerId);
            }
            
            condExpr.Evaluate(playerId);
            Thread.Sleep(EvaluateAfterMilliseconds);
        }
    }
}