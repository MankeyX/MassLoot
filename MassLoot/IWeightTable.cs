namespace MassLoot;

public interface IWeightTable
{
    /// <summary>
    /// Updates the weight at the specified index.
    /// </summary>
    void Update(
        int index,
        double weight
    );

    /// <summary>
    /// Selects the index of the item to drop based on the specified value.
    /// </summary>
    int SelectIndex(
        double value
    );
}