namespace MassLoot;

public class LootTable
{
    private readonly List<ILootItem> _loot;
    private readonly Dictionary<string, double> _variables;
    private readonly Dictionary<string, List<int>> _variablesToLootItemIndexesMap
        = new();

    private readonly BinaryIndexedWeightTable _binaryIndexedWeightTable;

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
        _binaryIndexedWeightTable = new BinaryIndexedWeightTable(_loot.Count);

        SortItemsByVariables();
        CalculateWeights();
        LinkVariablesToLootItems();

        for (var i = 0; i < _loot.Count; i++)
        {
            _binaryIndexedWeightTable.Update(i, _loot[i].Weight);
        }
    }

    /// <summary>
    /// Sort the items in the table by whether they have variables or not.
    /// </summary>
    private void SortItemsByVariables()
        => _loot.Sort(
            (item, _) => item.HasVariables ? 1 : 0
        );

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
            _binaryIndexedWeightTable.Update(index, _loot[index].Weight);
        }
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
        var index = _binaryIndexedWeightTable.SearchIndex(number);

        if (index < 0)
        {
            return LootItem.None;
        }

        var itemToDrop = _loot[index];

        return itemToDrop;
    }
}