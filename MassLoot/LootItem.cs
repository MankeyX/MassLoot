using MassLoot.Expressions;

namespace MassLoot;

public class LootItem(
    string itemId,
    string weightExpression
) : ILootItem, IEquatable<LootItem>
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

    public bool Equals(LootItem? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return
            ReferenceEquals(this, other) ||
            ItemId == other.ItemId;
    }

    public override bool Equals(object? obj)
        => obj is LootItem item && Equals(item);

    public override int GetHashCode()
        => ItemId.GetHashCode();
}