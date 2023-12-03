namespace TinyCiv.Server.Interpreter.Tokens;

public class TokenStream : ITokenStream
{
    private const string TokenSeparator = " ";

    private List<IToken>? _savedState;
    private Queue<IToken> _tokens;

    public TokenStream(string content)
    {
        _tokens = new Queue<IToken>();
        var rawTokens = content.Split(TokenSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var rawToken in rawTokens)
        {
            if (int.TryParse(rawToken, out var num))
            {
                _tokens.Enqueue(new NumberToken(num));
            }
            else
            {
                _tokens.Enqueue(new StringToken(rawToken));
            }
        }
    }

    public void SaveState()
    {
        _savedState = _tokens.ToList();
    }

    public bool IsEmpty()
    {
        return !_tokens.Any();
    }
    
    public void RestorePreviousState()
    {
        _tokens = new Queue<IToken>();
        foreach (var token in _savedState ?? throw new Exception())
        {
            _tokens.Enqueue(token);
        }
    }
    
    public IToken Next()
    {
        return _tokens.Dequeue();
    }
}