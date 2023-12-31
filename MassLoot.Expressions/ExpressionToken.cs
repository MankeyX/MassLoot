using System.Globalization;

namespace MassLoot.Expressions;

internal readonly record struct ExpressionToken
{
    public string Token { get; }
    public bool IsVariable { get; }

    /// <summary>
    /// <para>The operator represented by the token.</para>
    /// <para>Only valid if <see cref="IsOperator"/> is <see langword="true"/>.</para>
    /// </summary>
    public Operator Operator
        => (Operator)Token[0];

    private static readonly NumberFormatInfo Format = new()
    {
        NumberDecimalSeparator = ".",
        NegativeSign = "-"
    };

    public ExpressionToken(string Token)
    {
        this.Token = Token;

        IsVariable = TokenIsVariable();
    }

    /// <summary>
    /// Whether the token is a number.
    /// </summary>
    /// <param name="number">
    /// The number represented by the token.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the token is a number; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsNumber(out double number)
    {
        if (double.TryParse(Token, Format, out number))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Whether the token is an operator.
    /// </summary>
    /// <param name="op">
    /// The operator represented by the token.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the token is an operator; otherwise, <see langword="false"/>.
    /// </returns>
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

    /// <summary>
    /// Whether the token is a variable.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the token is neither a number nor an operator; otherwise, <see langword="false"/>.
    /// </returns>
    private bool TokenIsVariable() =>
        !IsNumber(out _) &&
        !IsOperator(out _);
}