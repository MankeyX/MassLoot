using System.Text.RegularExpressions;

namespace MassLoot.Expressions;

internal static partial class ExpressionParser
{
    private static readonly Dictionary<Operator, int> OrderOfOperations = new()
    {
        { Operator.Add, 1 },
        { Operator.Subtract, 1 },
        { Operator.Multiply, 2 },
        { Operator.Divide, 2 },
        { Operator.Exponentiate, 3 }
    };
    private static readonly char[] Separator = { ' ' };

    public static Expression Parse(string expression)
    {
        var operators = new Stack<ExpressionToken>();
        var output = new Stack<ExpressionToken>();

        var tokens = Tokenize(expression);

        if (tokens.Validate(out var errors))
        {
            foreach (var token in tokens)
            {
                if (token.IsOperator(out var @operator))
                {
                    switch (@operator)
                    {
                        case Operator.OpenParenthesis:
                            operators.Push(token);
                            break;
                        case Operator.ClosingParenthesis:
                            while (operators.Peek().IsOperator(out var op) &&
                                   op != Operator.OpenParenthesis)
                            {
                                var operatorValue = operators.Pop();
                                if (operatorValue.Operator != Operator.OpenParenthesis &&
                                    operatorValue.Operator != Operator.ClosingParenthesis)
                                {
                                    output.Push(operatorValue);
                                }
                            }
                            operators.Pop();
                            break;
                        default:
                            // Does op have a higher precedance than token[0]?
                            if (operators.TryPeek(out var opToken) &&
                                OrderOfOperations.TryGetValue(opToken.Operator, out var op1Precedence) &&
                                OrderOfOperations.TryGetValue(@operator, out var op2Precedence) &&
                                op1Precedence >= op2Precedence)
                            {
                                operators.Pop();
                                output.Push(opToken);
                            }

                            operators.Push(token);
                            break;
                    }
                }
                else
                {
                    output.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                var op = operators.Pop();
                output.Push(op);
            }
        }
        else
        {
            throw new InvalidOperationException(
                string.Join("\\n", errors.Select(x => x.Message))
            );
        }

        return new Expression(
            new ExpressionTokens(
                output.Reverse().ToList()
            )
        );
    }

    private static ExpressionTokens Tokenize(
        string expression
    )
    {
        var newExpression = expression.Replace(
            " ",
            string.Empty
        );

        newExpression =
            NegativeNumbersRegex()
                .Replace(
                    newExpression,
                    " $0"
                );

        newExpression =
            OperatorRegex()
                .Replace(
                    newExpression,
                    " $0 "
                );

        var tokens =
            newExpression.Split(
                Separator,
                StringSplitOptions.RemoveEmptyEntries
                | StringSplitOptions.TrimEntries
            );

        return
            new ExpressionTokens(
                tokens
                    .Select(x => new ExpressionToken(x))
                    .ToList()
            );
    }

    [GeneratedRegex(@"(?<=[ \^\-\*\+\(\/])(-(?:\w+))")]
    private static partial Regex NegativeNumbersRegex();

    [GeneratedRegex(@"([\^\*\+\(\)\/])|(?<=[\w|\)])(-)")]
    private static partial Regex OperatorRegex();
}