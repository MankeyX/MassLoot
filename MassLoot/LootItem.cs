using MassLoot.Expressions;

namespace MassLoot;

public record LootItem
{
    public string ItemId { get; }
    public string WeightExpression { get; }

    private double? _weight;
    public double Weight
    {
        get => _weight ??= Calculate();
    }

    private Expression Expression { get; }

    public LootItem(
        string itemId,
        string weightExpression
    )
    {
        ItemId = itemId;
        WeightExpression = weightExpression;

        Expression =
            ExpressionParser.Parse(
                weightExpression
            );
    }

    private double Calculate()
    {
        return Expression.Calculate();
    }
}