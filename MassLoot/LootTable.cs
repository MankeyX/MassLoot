namespace MassLoot;

public class LootTable
{
    private readonly List<ILootItem> _loot;
    private readonly Dictionary<string, double> _variables;
    private readonly Dictionary<string, List<int>> _variablesToLootItemIndexesMap
        = new();
    private CumulativeWeightTable _cumulativeWeightTable;

    private List<ILootItem> _currentLoot;

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

        InitializeWeightTable();
    }

    private void InitializeWeightTable()
    {
        FilterAndSortCurrentLoot();

        _cumulativeWeightTable = new CumulativeWeightTable(
            _loot
        );

        _cumulativeWeightTable.UpdateWeight(
            _currentLoot
        );
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
        }

        FilterAndSortCurrentLoot();
        _cumulativeWeightTable.UpdateWeight(
            _currentLoot
        );
    }

    /// <summary>
    /// Filter and sort the current loot based on the weight of the items.
    /// </summary>
    private void FilterAndSortCurrentLoot()
    {
        _currentLoot =
            _loot
                .Where(x => x.Weight > 0)
                .OrderBy(x => x.Weight)
                .ToList();
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

        if (index < 0)
        {
            return LootItem.None;
        }

        var itemToDrop = _currentLoot[index];

        return itemToDrop;
    }
}