namespace TinyCiv.Server.Interpreter.Tokens;

public interface ITokenStream
{
    IToken Next();
}