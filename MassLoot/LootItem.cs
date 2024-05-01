using System.Globalization;
using MassLoot.Expressions;
using MassLoot.Utilities;

namespace MassLoot;

public class LootItem(
    string itemId,
    string weightExpression
) : ILootItem, IEquatable<LootItem>
{
    /// <summary>
    /// An item that represents nothing.
    /// </summary>
    public static readonly LootItem None
        = Nothing(0);

    /// <summary>
    /// Create a loot item that represents nothing with the specified weight.
    /// </summary>
    public static LootItem Nothing(double weight)
        => new("None", weight.ToString(CultureInfo.InvariantCulture));

    public string ItemId { get; } = itemId;

    public double Weight { get; private set; }

    private readonly Expression _expression =
        ExpressionParser.Parse(
            weightExpression
        ).Match(
            left => throw new ArgumentException(string.Join(", ", left.Select(x => x.Message))),
            right => right
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