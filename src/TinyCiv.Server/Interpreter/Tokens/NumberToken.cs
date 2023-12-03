namespace TinyCiv.Server.Interpreter.Tokens;

public class NumberToken : IToken
{
    public int Value { get; }

    public NumberToken(int value)
    {
        Value = value;
    }
}