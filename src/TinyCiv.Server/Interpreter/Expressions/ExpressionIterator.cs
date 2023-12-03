using TinyCiv.Server.Core.Iterators;

namespace TinyCiv.Server.Interpreter.Expressions;

public class ExpressionIterator : IIterator<IExpression?>
{
    private readonly List<IExpression> _expressions;

    private int _index;

    public ExpressionIterator(List<IExpression> expressions)
    {
        _expressions = expressions;
        _index = 0;
    }
    
    public IExpression? Next()
    {
        return HasNext() ? _expressions[_index++] : null;
    }

    public bool HasNext()
    {
        return _expressions?.Count > _index;
    }
}