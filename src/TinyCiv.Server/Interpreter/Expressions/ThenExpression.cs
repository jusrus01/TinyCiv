using TinyCiv.Server.Interpreter.Tokens;

namespace TinyCiv.Server.Interpreter.Expressions;

public class ThenExpression : IExpression
{
    private const string ExpressionActivationKey = "then";
    
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

        return true;
    }

    public void Evaluate(Guid playerId)
    {
    }
}