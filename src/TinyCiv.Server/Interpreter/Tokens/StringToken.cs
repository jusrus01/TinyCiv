namespace TinyCiv.Server.Interpreter.Tokens;

public class StringToken : IToken
{
    public string Value { get; }

    public StringToken(string value)
    {
        Value = value;
    }
}