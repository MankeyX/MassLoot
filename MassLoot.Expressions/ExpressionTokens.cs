using System.Collections;
using System.Globalization;

namespace MassLoot.Expressions;

internal record ExpressionTokens(
    List<ExpressionToken> Tokens
) : IReadOnlyList<ExpressionToken>
{
    private List<ExpressionToken> Tokens { get; } = Tokens;

    public int Count
        => Tokens.Count;

    public ExpressionToken this[int index]
        => Tokens[index];

    public IEnumerator<ExpressionToken> GetEnumerator()
    {
        return Tokens.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal readonly record struct ExpressionToken(
    string Token
)
{
    public string Token { get; } = Token;

    public Operator Operator
        => (Operator)Token[0];

    public bool IsOperator(out Operator op)
    {
        if (Token.Length > 1)
        {
            op = Operator.None;
            return false;
        }

        op = (Operator)Token[0];
        return
            Enum.IsDefined(op);
    }

    private static readonly NumberFormatInfo Format = new()
    {
        NumberDecimalSeparator = ".",
        NegativeSign = "-"
    };

    public bool IsNumber(out double number)
    {
        if (double.TryParse(Token, Format, out number))
        {
            return true;
        }

        return false;
    }

    public bool IsVariable()
    {
        return !IsOperator(out _) &&
               !IsNumber(out _);
    }
}