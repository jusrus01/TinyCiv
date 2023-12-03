using TinyCiv.Server.Interpreter.Tokens;

namespace TinyCiv.Server.Interpreter.Expressions;

public interface IExpression
{
    bool Parse(ITokenStream stream);
    void Evaluate(Guid playerId);
}