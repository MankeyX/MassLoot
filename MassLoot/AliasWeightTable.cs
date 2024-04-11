namespace MassLoot;

/// <summary>
/// A weight table that uses the alias method to select indexes. Great for when you have a large number of items.
/// </summary>
public class AliasWeightTable : IWeightTable
{
    private record AliasNode(
        double Probability,
        int Alias
    );

    private static readonly Random Random = new();

    private readonly AliasNode[] _aliasTable;

    public AliasWeightTable(
        IReadOnlyList<ILootItem> items
    )
    {
        _aliasTable = new AliasNode[items.Count];

        Initialize(items);
    }

    private void Initialize(
        IReadOnlyList<ILootItem> items
    )
    {
        var sum = items.Sum(item => item.Weight);
        var small = new Stack<int>(items.Count);
        var large = new Stack<int>(items.Count);
        var probabilities = Enumerable.Repeat(0d, items.Count).ToArray();

        for (var i = 0; i < items.Count; i++)
        {
            probabilities[i] = items[i].Weight * items.Count / sum;
            if (probabilities[i] < 1)
            {
                small.Push(i);
            }
            else
            {
                large.Push(i);
            }
        }

        while (small.Count > 0 && large.Count > 0)
        {
            var s = small.Pop();
            var l = large.Pop();

            _aliasTable[s] =
                new AliasNode(probabilities[s], l);

            probabilities[l] = probabilities[l] + probabilities[s] - 1;
            if (probabilities[l] < 1)
            {
                small.Push(l);
            }
            else
            {
                large.Push(l);
            }
        }

        while (large.Count != 0)
        {
            var g = large.Pop();

            _aliasTable[g] =
                new AliasNode(1, g);
        }

        while (small.Count != 0)
        {
            var s = small.Pop();

            _aliasTable[s] =
                new AliasNode(1, s);
        }
    }

    /// <summary>
    /// Updating is not supported by this table.
    /// </summary>
    public void Update(int index, double weight) { }

    /// <inheritdoc cref="IWeightTable.SelectIndex" />
    public int SelectIndex(
        double number
    )
    {
        // Get a random index.
        var index = Random.Next(_aliasTable.Length);

        // Select either from the "high" or "low" portion of the probability distribution.
        return number <= _aliasTable[index].Probability
            ? index
            : _aliasTable[index].Alias;
    }
};