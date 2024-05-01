namespace MassLoot.Expressions;

internal record Expression(
    ExpressionTokens Tokens
)
{
    /// <summary>
    /// Whether the expression contains variables.
    /// </summary>
    public bool HasVariables
        => Tokens.HasVariables;

    /// <summary>
    /// Calculate the value of the expression.
    /// </summary>
    /// <param name="variables">
    /// The variables to be made available to the expression.
    /// </param>
    /// <returns>
    /// The value of the expression.
    /// </returns>
    public double Calculate(
        IReadOnlyDictionary<string, double>? variables = null
    )
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
                var value2 = stack.Pop();
                var value1 = stack.Pop();
                stack.Push(
                    CalculateOperation(operand, value1, value2)
                );
            }
            else if (token.IsVariable)
            {
                if (variables is not null &&
                    variables.TryGetValue(token.Token, out var value))
                {
                    stack.Push(value);
                }
                else
                {
                    stack.Push(0);
                }
            }
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
            _ => op1
        };
    }

    /// <summary>
    /// Get the variables used in the expression.
    /// </summary>
    /// <returns>
    /// The variables used in the expression.
    /// </returns>
    public IEnumerable<string> GetVariables()
    {
        return Tokens
            .Where(token => token.IsVariable)
            .Select(token => token.Token);
    }
}