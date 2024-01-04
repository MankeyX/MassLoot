using MassLoot.Expressions;

namespace MassLoot;

public class LootItem : ILootItem
{
    public string ItemId { get; }

    public double Weight { get; private set; }

    private readonly Expression _expression;

    public LootItem(
        string itemId,
        string weightExpression
    )
    {
        ItemId = itemId;

        _expression =
            ExpressionParser.Parse(
                weightExpression
            );
    }

    public bool HasVariables
        => _expression.HasVariables;

    /// <inheritdoc />
    public IEnumerable<string> GetVariables()
        => _expression.GetVariables();

    /// <inheritdoc />
    public void Calculate(
        IReadOnlyDictionary<string, double> variables
    ) =>
        Weight = _expression.Calculate(variables);
}