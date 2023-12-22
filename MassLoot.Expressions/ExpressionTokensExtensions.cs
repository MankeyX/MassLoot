namespace MassLoot.Expressions;

internal static class ExpressionTokensExtensions
{
    public static bool Validate(
        this ExpressionTokens tokens,
        out ValidationError[] errors
    )
    {
        errors = Array.Empty<ValidationError>();
        var validations = new List<ValidationError>();

        if (tokens.Count == 0)
        {
            validations.Add(
                new ValidationError(
                    "An expression cannot be empty"
                )
            );
        }

        if (tokens.First().IsOperator(out var startingOp) &&
            startingOp != Operator.OpenParenthesis)
        {
            validations.Add(
                new ValidationError(
                    "An expression cannot begin with an operator"
                )
            );
        }

        if (tokens[^1].IsOperator(out var endingOp) &&
            endingOp != Operator.ClosingParenthesis)
        {
            validations.Add(
                new ValidationError(
                    "An expression cannot end with an operator"
                )
            );
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
                        if (closingParenthesis > openParenthesis)
                        {
                            validations.Add(
                                new ValidationError(
                                    "Closing parenthesis cannot be used before opening parenthesis"
                                )
                            );
                        }

                        closingParenthesis++;
                        break;
                    default:
                    {
                        if (i > 0)
                        {
                            if (tokens[i - 1].IsOperator(out var opBefore) &&
                                opBefore != Operator.ClosingParenthesis)
                            {
                                validations.Add(
                                    new ValidationError(
                                        "Consecutive operators are not allowed"
                                    )
                                );
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
                    "Mismatched parenthesis in expression"
                )
            );
        }

        errors = validations.ToArray();

        return
            validations.Count == 0;
    }
}