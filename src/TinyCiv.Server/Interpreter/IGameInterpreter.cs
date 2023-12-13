namespace TinyCiv.Server.Interpreter;

public interface IGameInterpreter
{
    void Interpret(Guid playerId, string? content);
}