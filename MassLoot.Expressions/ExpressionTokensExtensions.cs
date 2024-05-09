using MassLoot.Utilities;

namespace MassLoot.Expressions;

internal static class ExpressionTokensExtensions
{
    /// <summary>
    /// Validates the expression tokens.
    /// </summary>
    /// <returns>
    /// The expression tokens if they are valid; otherwise, an array of validation errors.
    /// </returns>
    public static Either<ValidationError[], ExpressionTokens> Validate(
        this ExpressionTokens tokens
    )
    {
        var validations = new List<ValidationError>();

        if (tokens.Count == 0)
        {
            validations.Add(
                new ValidationError(
                    ValidationErrorType.EmptyExpression,
                    "An expression cannot be empty"
                )
            );

            return validations.ToArray();
        }

        var openParenthesis = 0;
        var closingParenthesis = 0;
        for (var i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].IsOperator(out var op))
            {
                switch (op)
                {
                    case Operator.OpenParenthesis:
                        openParenthesis++;
                        break;
                    case Operator.ClosingParenthesis:
                        closingParenthesis++;

                        if (closingParenthesis > openParenthesis)
                        {
                            validations.Add(
                                new ValidationError(
                                    ValidationErrorType.MalformedExpression,
                                    $"Closing parenthesis cannot be used before opening parenthesis: '{tokens}'"
                                )
                            );
                        }

                        break;
                    default:
                    {
                        switch (i)
                        {
                            case 0:
                                validations.Add(
                                    new ValidationError(
                                        ValidationErrorType.MalformedExpression,
                                        $"An expression cannot begin with an operator: '{tokens}'"
                                    )
                                );

                                break;
                            case > 0:
                            {
                                if (tokens[i - 1].IsOperator(out var opBefore) &&
                                    opBefore != Operator.ClosingParenthesis)
                                {
                                    validations.Add(
                                        new ValidationError(
                                            ValidationErrorType.MalformedExpression,
                                            $"Consecutive operators are not allowed: '{tokens}'"
                                        )
                                    );
                                }

                                break;
                            }
                        }

                        break;
                    }
                }
            }
        }

        if (openParenthesis != closingParenthesis)
        {
            validations.Add(
                new ValidationError(
                    ValidationErrorType.MalformedExpression,
                    $"Mismatched parenthesis in expression: '{tokens}'"
                )
            );
        }

        if (tokens[^1].IsOperator(out var endingOp) &&
            endingOp != Operator.ClosingParenthesis)
        {
            validations.Add(
                new ValidationError(
                    ValidationErrorType.MalformedExpression,
                    $"An expression cannot end with an operator: '{tokens}'"
                )
            );
        }

        return validations.Count > 0
            ? validations.ToArray()
            : tokens;
    }
}