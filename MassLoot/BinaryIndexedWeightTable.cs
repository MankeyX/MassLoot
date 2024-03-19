namespace MassLoot;

public class BinaryIndexedWeightTable(int size)
{
    private readonly double[] _values = new double[size + 1];
    private readonly double[] _tree = new double[size + 1];

    public void Update(
        int index,
        double value
    )
    {
        index++;
        var initialIndex = index;
        var initialValue = _values[index];

        var delta = value - _values[index];
        _values[index] = value;

        while (index < _tree.Length)
        {
            if (index == initialIndex && delta + initialValue == 0)
            {
                _tree[index] = 0;
            }
            else
            {
                _tree[index] += delta;
            }
            index += index & -index;
        }
    }

    public double PrefixSum(
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

    public int SearchIndex(
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