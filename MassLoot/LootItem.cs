using MassLoot.Expressions;

namespace MassLoot;

public class LootItem(
    string itemId,
    string weightExpression
) : ILootItem
{
    public static readonly LootItem None = new("None", "0");

    public string ItemId { get; } = itemId;

    public double Weight { get; private set; }

    private readonly Expression _expression =
        ExpressionParser.Parse(
            weightExpression
        );

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