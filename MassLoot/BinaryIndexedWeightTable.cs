namespace MassLoot;

/// <summary>
/// A weight table that uses a binary indexed tree to update and select indexes.
/// </summary>
public class BinaryIndexedWeightTable : IWeightTable
{
    private readonly double[] _values;
    private readonly double[] _tree;

    public BinaryIndexedWeightTable(
        IReadOnlyList<ILootItem> weights
    )
    {
        _values = new double[weights.Count + 1];
        _tree = new double[weights.Count + 1];

        for (var i = 0; i < weights.Count; i++)
        {
            Update(i, weights[i].Weight);
        }
    }

    /// <inheritdoc cref="IWeightTable.Update" />
    public void Update(
        int index,
        double weight
    )
    {
        index++;

        var delta = weight - _values[index];
        _values[index] = weight;

        while (index < _tree.Length)
        {
            _tree[index] += delta;
            index += index & -index;
        }
    }

    /// <summary>
    /// Returns the prefix sum up to the specified index.
    /// </summary>
    private double PrefixSum(
        int index
    )
    {
        index++;
        var sum = 0d;
        while (index > 0)
        {
            sum += _tree[index];
            index -= index & -index;
        }

        return sum;
    }

    /// <inheritdoc cref="IWeightTable.SelectIndex" />
    public int SelectIndex(
        double value
    )
    {
        var totalWeight = PrefixSum(_tree.Length - 2);
        var target = totalWeight * value;

        var low = 0;
        var high = _tree.Length - 2;
        while (low < high)
        {
            var mid = (low + high) / 2;
            if (target <= PrefixSum(mid))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }
}