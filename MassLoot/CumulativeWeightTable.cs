namespace MassLoot;

public class CumulativeWeightTable
{
    private readonly List<double> _cumulativeWeights;

    public CumulativeWeightTable(
        List<LootItem> loot
    )
    {
        _cumulativeWeights = new List<double>(loot.Count);

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
        int index,
        List<LootItem> lootItems
    )
    {
        var cumulativeWeight = index == 0
            ? 0d
            : _cumulativeWeights[index - 1];

        do
        {
            cumulativeWeight += lootItems[index].Weight;
            _cumulativeWeights[index] = cumulativeWeight;
            index++;
        }
        while (index < lootItems.Count);
    }

    /// <summary>
    /// Selects the index of the item to drop based on the specified number.
    /// </summary>
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