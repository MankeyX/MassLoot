namespace MassLoot;

public class CumulativeWeightTable
{
    private readonly List<LootItem> _loot;
    private readonly List<double> _cumulativeWeights;

    public CumulativeWeightTable(
        List<LootItem> loot
    )
    {
        _loot = loot;
        _cumulativeWeights = new List<double>(loot.Count);

        var cumulativeWeight = 0d;
        foreach (var item in loot)
        {
            cumulativeWeight += item.Weight;
            _cumulativeWeights.Add(cumulativeWeight);
        }
    }

    /// <summary>
    /// Selects an item from the table.
    /// </summary>
    /// <param name="randomNumber">
    /// A random number between 0 and 1.
    /// </param>
    public LootItem Drop(
        double randomNumber
    )
    {
        randomNumber *= _cumulativeWeights[^1];

        var index = _cumulativeWeights.BinarySearch(randomNumber);
        if (index < 0)
        {
            index = ~index;
        }

        return index < _cumulativeWeights.Count
            ? _loot[index]
            : _loot[^1];
    }
}