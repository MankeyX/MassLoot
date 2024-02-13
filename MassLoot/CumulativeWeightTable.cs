namespace MassLoot;

public class CumulativeWeightTable
{
    private List<double> _cumulativeWeights;

    public CumulativeWeightTable(
        IReadOnlyCollection<ILootItem> loot
    )
    {
        if (loot.Count == 0)
        {
            throw new ArgumentException(
                "You cannot create a cumulative weights-table with zero items.",
                nameof(loot)
            );
        }

        _cumulativeWeights = new List<double>(loot.Count);

        InitializeTable(loot);
    }

    private void InitializeTable(
        IEnumerable<ILootItem> loot
    )
    {
        var cumulativeWeight = 0d;
        foreach (var item in loot)
        {
            cumulativeWeight += item.Weight;
            _cumulativeWeights.Add(cumulativeWeight);
        }
    }

    /// <summary>
    /// Updates the cumulative weights starting from the specified index.
    /// </summary>
    /// <param name="index">
    /// The index to start updating the cumulative weights from.
    /// </param>
    /// <param name="lootItems">
    /// The loot items to update the cumulative weights with.
    /// </param>
    public void UpdateWeight(
        IList<ILootItem> lootItems
    )
    {
        _cumulativeWeights = new List<double>(lootItems.Count);
        InitializeTable(
            lootItems
        );
    }

    /// <summary>
    /// Selects the index of the item to drop based on the specified number.
    /// </summary>
    /// <returns>
    /// The index of the item that was selected.
    /// </returns>
    public int SelectIndex(
        double number
    )
    {
        number *= _cumulativeWeights[^1];

        var index = _cumulativeWeights.BinarySearch(number);
        if (index < 0)
        {
            index = ~index;
        }

        return index < _cumulativeWeights.Count
            ? index
            : _cumulativeWeights.Count - 1;
    }
}