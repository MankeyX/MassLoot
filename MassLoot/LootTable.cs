using MassLoot.Expressions;

namespace MassLoot;

public class LootTable : ILootItem
{
    private readonly List<ILootItem> _loot;
    private readonly Dictionary<string, double> _variables;
    private readonly Dictionary<string, List<int>> _variablesToLootItemIndexesMap
        = new();
    private readonly CumulativeWeightTable _cumulativeWeightTable;

    public LootTable(
        IReadOnlyCollection<ILootItem> loot,
        IReadOnlyDictionary<string, double> variables
    )
    {
        if (loot.Count == 0)
        {
            throw new ArgumentException(
                "You cannot create a loot table with zero items.",
                nameof(loot)
            );
        }

        _loot = loot.ToList();
        _variables = variables.ToDictionary();

        SortItemsByVariables();
        CalculateWeights();
        LinkVariablesToLootItems();

        _cumulativeWeightTable = new CumulativeWeightTable(loot);
    }

    /// <summary>
    /// Sort the items in the table by whether they have variables or not.
    /// </summary>
    private void SortItemsByVariables()
    {
        _loot.Sort((item, _) => item.HasVariables ? 1 : 0);
    }

    /// <summary>
    /// Calculate the weight of all items in the table.
    /// </summary>
    private void CalculateWeights()
    {
        foreach (var lootItem in _loot)
        {
            lootItem.Calculate(_variables);
        }
    }

    /// <summary>
    /// Link variables to loot items that can be used to update affected items.
    /// </summary>
    private void LinkVariablesToLootItems()
    {
        for (var index = 0; index < _loot.Count; index++)
        {
            var lootItem = _loot[index];
            var variables = lootItem.GetVariables();
            foreach (var variable in variables)
            {
                if (!_variablesToLootItemIndexesMap.TryGetValue(
                        variable,
                        out var lootItemIndexes
                    ))
                {
                    lootItemIndexes = new List<int>();
                    _variablesToLootItemIndexesMap[variable] = lootItemIndexes;
                }

                lootItemIndexes.Add(index);
            }
        }
    }

    /// <summary>
    /// Update the value of a variable and recalculate the weight of items that use it in their expression.
    /// </summary>
    /// <param name="variable">
    /// The name of the variable to update.
    /// </param>
    /// <param name="value">
    /// The new value of the variable.
    /// </param>
    public void UpdateVariable(
        string variable,
        double value
    )
    {
        _variables[variable] = value;

        if (!_variablesToLootItemIndexesMap.TryGetValue(
                variable,
                out var lootItemIndexes
            ))
        {
            return;
        }

        foreach (var index in lootItemIndexes)
        {
            _loot[index].Calculate(_variables);
        }

        var firstIndex = lootItemIndexes.First();
        _cumulativeWeightTable.UpdateWeight(
            firstIndex,
            _loot
        );
    }

    /// <summary>
    /// Drop an item from the table based on the specified number.
    /// </summary>
    /// <returns>
    /// The item that was dropped.
    /// </returns>
    public ILootItem Drop(
        double number
    )
    {
        var index = _cumulativeWeightTable.SelectIndex(number);
        var itemToDrop = _loot[index];

        if (itemToDrop is LootTable table)
        {
            number = GetAdjustedNumberForNestedTable(number, index);

            itemToDrop = table.Drop(number);
        }

        return itemToDrop;
    }

    private double GetAdjustedNumberForNestedTable(double number, int index)
    {
        var totalWeight = _cumulativeWeightTable.GetWeightOfTable();
        var weightBefore = _cumulativeWeightTable.GetCumulativeWeightBefore(index);
        var itemWeight = _loot[index].Weight;

        // Calculate the start and end range of the number for the selected item
        var startRange = weightBefore / totalWeight;
        var endRange = (weightBefore + itemWeight) / totalWeight;

        // Normalize the number to the range of the selected item in the nested loot table
        return (number - startRange) / (endRange - startRange);
    }

    #region ILootItem

    public string ItemId { get; }
    public double Weight { get; private set; }

    private readonly Expression _expression;

    public LootTable(
        string itemId,
        string weightExpression,
        IReadOnlyCollection<ILootItem> loot,
        IReadOnlyDictionary<string, double> variables
    ) : this(loot, variables)
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

    #endregion
}