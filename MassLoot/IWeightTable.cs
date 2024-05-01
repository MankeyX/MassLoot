namespace MassLoot;

public interface IWeightTable
{
    /// <summary>
    /// Initializes the weight table with the specified items.
    /// </summary>
    void Initialize(
        IReadOnlyList<IWeightedItem> items
    );

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
    /// <returns>
    /// The index of the item to drop. A value of -1 indicates that no item was selected.
    /// </returns>
    int SelectIndex(
        double value
    );
}