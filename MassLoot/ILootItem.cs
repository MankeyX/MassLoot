namespace MassLoot;

public interface ILootItem
{
    string ItemId { get; }
    double Weight { get; }
    bool HasVariables { get; }

    /// <summary>
    /// Get the variables used in the weight expression.
    /// </summary>
    /// <returns>
    /// The variables used in the weight expression.
    /// </returns>
    IEnumerable<string> GetVariables();

    /// <summary>
    /// Calculate the weight of the item.
    /// The weight can then be retrieved with the <see cref="Weight"/> property.
    /// </summary>
    /// <param name="variables">
    /// The variables to be made available to the expression.
    /// </param>
    void Calculate(IReadOnlyDictionary<string, double> variables);
}