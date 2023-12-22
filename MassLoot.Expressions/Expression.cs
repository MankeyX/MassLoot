namespace MassLoot.Expressions;

internal record Expression(
    ExpressionTokens Tokens
)
{
    public double Calculate()
    {
        var stack = new Stack<double>();

        foreach (var token in Tokens)
        {
            if (token.IsNumber(out var number))
            {
                stack.Push(number);
            }
            else if (token.IsOperator(out var operand))
            {
                if (stack.Count < 2)
                {
                    throw new InvalidOperationException(
                        $"Not enough operands for operator {token}"
                    );
                }

                var value2 = stack.Pop();
                var value1 = stack.Pop();
                stack.Push(
                    CalculateOperation(operand, value1, value2)
                );
            }
        }

        if (stack.Count != 1)
        {
            throw new InvalidOperationException(
                "The expression is malformed."
            );
        }

        return stack.Pop();
    }

    private static double CalculateOperation(
        Operator token,
        double op1,
        double op2
    )
    {
        return token switch
        {
            Operator.Add => op1 + op2,
            Operator.Subtract => op1 - op2,
            Operator.Multiply => op1 * op2,
            Operator.Divide => op1 / op2,
            Operator.Exponentiate => Math.Pow(op1, op2),
            _ => throw new InvalidOperationException($"{token} is an invalid operator")
        };
    }
};