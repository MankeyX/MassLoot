using MassLoot.Expressions;

namespace MassLoot;

public record LootItem
{
    public string ItemId { get; }
    public string WeightExpression { get; }

    public double Weight { get; private set; }

    private readonly Expression _expression;

    public LootItem(
        string itemId,
        string weightExpression
    )
    {
        ItemId = itemId;
        WeightExpression = weightExpression;

        _expression =
            ExpressionParser.Parse(
                weightExpression
            );
    }

    /// <summary>
    /// Get the variables used in the weight expression.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetVariables()
        => _expression.GetVariables();

    /// <summary>
    /// Calculate the weight of the item.
    /// The weight can then be retrieved with the <see cref="Weight"/> property.
    /// </summary>
    /// <param name="variables">
    /// The variables to be made available to the expression.
    /// </param>
    public void Calculate(
        IReadOnlyDictionary<string, double> variables
    ) =>
        Weight = _expression.Calculate(variables);
}