using System.Text.RegularExpressions;
using MassLoot.Utilities;
using static MassLoot.Utilities.Either;

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
    private static readonly char[] Separator = [' '];

    /// <summary>
    /// Parses the expression into an <see cref="Expression"/> object.
    /// </summary>
    /// <param name="expression">
    /// The expression to parse.
    /// </param>
    /// <returns>
    /// The parsed expression or validation errors describing why parsing failed.
    /// </returns>
    public static Either<ValidationError[], Expression> Parse(string expression)
        => Tokenize(expression)
            .Validate()
            .Match(
                Left<ValidationError[], Expression>,
                InternalParse
            );

    /// <summary>
    /// Parse the tokens and return the expression.
    /// </summary>
    private static Either<ValidationError[], Expression> InternalParse(
        ExpressionTokens tokens
    )
    {
        var operators = new Stack<ExpressionToken>();
        var output = new Stack<ExpressionToken>();

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
                        // Does op have a higher precedence than token[0]?
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

        return new Expression(
            new ExpressionTokens(
                output.Reverse().ToList()
            )
        );
    }

    /// <summary>
    /// Tokenizes the expression by using two regexes to space out and split the expression into tokens.
    /// </summary>
    /// <returns>
    /// The tokenized expression.
    /// </returns>
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

    /// <summary>
    /// Matches negative numbers that are not preceded by an operator.
    /// </summary>
    [GeneratedRegex(@"(?<=[ \^\-\*\+\(\/])(-(?:\w+))")]
    private static partial Regex NegativeNumbersRegex();

    /// <summary>
    /// Matches operators.
    /// </summary>
    [GeneratedRegex(@"([\^\*\+\(\)\/])|(?<=[\w|\)])(-)")]
    private static partial Regex OperatorRegex();
}