using MassLoot.Utilities;

namespace MassLoot;

public interface IWeightedItem
{
    double Weight { get; }
}

public interface ILootItem : IWeightedItem
{
    string ItemId { get; }
    bool HasVariables { get; }

    /// <summary>
    /// Get the variables used in the weight expression.
    /// </summary>
    IEnumerable<string> GetVariables();

    /// <summary>
    /// Initialize the item and calculate the weight.
    /// </summary>
    /// <returns>
    /// A <see cref="Either{T1,T2}"/> that contains any validation errors
    /// or <see cref="Unit"/>.<see cref="Unit.Default"/> which indicates successful initialization.
    /// </returns>
    Either<ValidationError[], Unit> Initialize(
        IReadOnlyDictionary<string, double> variables
    );

    /// <summary>
    /// Calculate the weight of the item.
    /// The weight can then be retrieved with the <see cref="Weight"/> property.
    /// </summary>
    /// <param name="variables">
    /// The variables to be made available to the expression.
    /// </param>
    void Calculate(IReadOnlyDictionary<string, double> variables);
}