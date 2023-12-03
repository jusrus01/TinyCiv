using TinyCiv.Server.Interpreter.Expressions;
using TinyCiv.Server.Interpreter.Tokens;

namespace TinyCiv.Server.Interpreter;

public class GameInterpreter : IGameInterpreter
{
    private readonly IExpressionStorage _storage;

    public GameInterpreter(IExpressionStorage storage)
    {
        _storage = storage;
    }

    public void Interpret(Guid playerId, string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new Exception("Invalid input to interpreter");
        }

        var stream = new TokenStream(content);
        var parsedExpressions = new List<IExpression>();

        var iterator = _storage.GetIterator();
        while (iterator.HasNext())
        {
            var expr = iterator.Next();
            ArgumentNullException.ThrowIfNull(expr);
            
            stream.SaveState();
            if (!expr.Parse(stream))
            {
                stream.RestorePreviousState();
            }
            else
            {
                parsedExpressions.Add(expr);
            }

            if (stream.IsEmpty())
            {
                break;
            }
        }
        
        foreach (var successfullyParsedExpression in parsedExpressions)
        {
            successfullyParsedExpression.Evaluate(playerId);
        }
    }
}