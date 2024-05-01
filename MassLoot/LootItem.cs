using System.Globalization;
using MassLoot.Expressions;
using MassLoot.Utilities;
using static MassLoot.Utilities.UnionExtensions;

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

    private Expression _expression = null!;

    public bool HasVariables
        => _expression.HasVariables;

    private bool _initialized;
    /// <inheritdoc />
    public Union<ValidationError[], Unit> Initialize(
        IReadOnlyDictionary<string, double> variables
    ) =>
        ExpressionParser.Parse(
            weightExpression
        ).Match(
            left => left,
            right =>
            {
                if (_initialized)
                {
                    return Right<ValidationError[], Unit>(Unit.Default);
                }

                _expression = right;
                Calculate(variables);
                _initialized = true;

                return Right<ValidationError[], Unit>(Unit.Default);
            }
        );

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