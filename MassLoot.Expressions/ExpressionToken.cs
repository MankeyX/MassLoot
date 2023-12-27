using System.Globalization;

namespace MassLoot.Expressions;

internal readonly record struct ExpressionToken
{
    public string Token { get; }
    public bool IsVariable { get; }

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

    public bool IsNumber(out double number)
    {
        if (double.TryParse(Token, Format, out number))
        {
            return true;
        }

        return false;
    }

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

    private bool TokenIsVariable() =>
        !IsNumber(out _) &&
        !IsOperator(out _);
}