using TinyCiv.Server.Interpreter.Expressions;

namespace TinyCiv.Server.Interpreter;

public class ConditionContext
{
    public Guid PlayerId { get; set; }
    public Condition Condition  { get; set; }
    public int UnitCount { get; set; }
    public bool Result { get; set; }
}