namespace MassLoot;

public record AliasTable
{
    private record AliasNode(
        double Prob,
        int Alias
    );

    private static readonly Random Random = new();

    public string TableId { get; }
    public List<LootItem> Loot { get; }
    private readonly AliasNode[] _aliasTable;

    public AliasTable(
        string tableId,
        List<LootItem> loot
    )
    {
        TableId = tableId;
        Loot = loot;
        _aliasTable = new AliasNode[Loot.Count];

        ComputeAliasMethod();
    }

    private void ComputeAliasMethod()
    {
        var sum = Loot.Sum(item => item.Weight);
        var small = new Stack<int>(Loot.Count);
        var large = new Stack<int>(Loot.Count);
        var probabilities = Enumerable.Repeat(0d, Loot.Count).ToArray();

        for (var i = 0; i < Loot.Count; i++)
        {
            probabilities[i] = Loot[i].Weight * Loot.Count / sum;
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
                new AliasNode(
                    probabilities[s],
                    l
                );

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
                new AliasNode(
                    1,
                    g
                );
        }

        while (small.Count != 0)
        {
            var s = small.Pop();

            _aliasTable[s] =
                new AliasNode(
                    1,
                    s
                );
        }
    }

    public LootItem Drop()
    {
        // Get a random index and uniform random number.
        var i = Random.Next(Loot.Count);
        var r = Random.NextDouble();

        // Select either from the "high" or "low" portion of the probability distribution.
        return
            r <= _aliasTable[i].Prob
                ? Loot[i]
                : Loot[_aliasTable[i].Alias];
    }
};