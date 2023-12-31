using System.Collections;

namespace MassLoot.Expressions;

internal record ExpressionTokens
    : IReadOnlyList<ExpressionToken>
{
    private readonly List<ExpressionToken> _tokens;

    /// <summary>
    /// The number of tokens in the expression.
    /// </summary>
    public int Count
        => _tokens.Count;

    /// <summary>
    /// Indicates whether the expression contains variables.
    /// </summary>
    public bool HasVariables { get; }

    public ExpressionToken this[int index]
        => _tokens[index];

    public ExpressionTokens(
        List<ExpressionToken> tokens
    )
    {
        _tokens = tokens;

        HasVariables = _tokens.Any(token => token.IsVariable);
    }

    public IEnumerator<ExpressionToken> GetEnumerator()
    {
        return _tokens.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return string.Join(' ', _tokens.Select(x => x.Token));
    }
}