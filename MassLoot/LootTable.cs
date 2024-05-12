using System.Diagnostics.CodeAnalysis;
using MassLoot.Utilities;
using static MassLoot.Utilities.EitherExtensions;

namespace MassLoot;

public class LootTable<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    TWeightTable
> where TWeightTable : IWeightTable
{
    private IReadOnlyList<ILootItem> _loot = null!;
    private Dictionary<string, double> _variables = null!;
    private readonly Dictionary<string, List<int>> _variablesToLootItemIndexesMap
        = new();

    private IWeightTable _weightTable = null!;

    private LootTable() { }

    /// <summary>
    /// Initialize all properties and item weights
    /// </summary>
    private Either<List<ValidationError>, Unit> Initialize(
        IReadOnlyList<ILootItem> loot,
        Dictionary<string, double> variables
    ) =>
        Validate(loot)
            .Bind(_ => ValidateAndInitializeItems(loot, variables))
            .Map(_ => InitializeTable(loot, variables));

    /// <summary>
    /// Validate that the loot table will not be empty.
    /// </summary>
    private static Either<List<ValidationError>, Unit> Validate(
        IReadOnlyList<ILootItem> loot
    ) =>
        !loot.Any()
            ? new List<ValidationError>
            {
                new(
                    ValidationErrorType.EmptyTable,
                    "The loot table must contain at least one item."
                )
            }
            : Unit.Default;

    /// <summary>
    /// Validate and calculate the weight of all items in the table.
    /// </summary>
    private static Either<List<ValidationError>, Unit> ValidateAndInitializeItems(
        IEnumerable<ILootItem> loot,
        IReadOnlyDictionary<string, double> variables
    )
    {
        var validationErrors = new List<ValidationError>();

        foreach (var lootItem in loot)
        {
            lootItem.Initialize(variables)
                .Match(
                    left => validationErrors.AddRange(left),
                    _ => lootItem.Calculate(variables)
                );
        }

        return validationErrors.Count > 0
            ? validationErrors
            : Unit.Default;
    }

    /// <summary>
    /// Finish initializing the table.
    /// </summary>
    private Unit InitializeTable(
        IReadOnlyList<ILootItem> loot,
        Dictionary<string, double> variables
    )
    {
        _variables = variables;
        _loot = loot.OrderBy(x => x.HasVariables).ToList();

        LinkVariablesToLootItems();

        _weightTable = Activator.CreateInstance<TWeightTable>();
        _weightTable.Initialize(_loot);

        return Unit.Default;
    }

    /// <summary>
    /// Link variables to loot items that can be used to update affected items.
    /// </summary>
    private void LinkVariablesToLootItems()
    {
        for (var index = 0; index < _loot.Count; index++)
        {
            var lootItem = _loot[index];
            var variables = lootItem.GetVariables();
            foreach (var variable in variables)
            {
                if (!_variablesToLootItemIndexesMap.TryGetValue(
                        variable,
                        out var lootItemIndexes
                    ))
                {
                    lootItemIndexes = new List<int>();
                    _variablesToLootItemIndexesMap[variable] = lootItemIndexes;
                }

                lootItemIndexes.Add(index);
            }
        }
    }

    /// <summary>
    /// Update the value of a variable and recalculate the weight of items that use it in their expression.
    /// </summary>
    /// <param name="variable">
    /// The name of the variable to update.
    /// </param>
    /// <param name="value">
    /// The new value of the variable.
    /// </param>
    public void UpdateVariable(
        string variable,
        double value
    )
    {
        _variables[variable] = value;

        if (!_variablesToLootItemIndexesMap.TryGetValue(
                variable,
                out var lootItemIndexes
            ))
        {
            return;
        }

        foreach (var index in lootItemIndexes)
        {
            _loot[index].Calculate(_variables);
            _weightTable.Update(index, _loot[index].Weight);
        }
    }

    /// <summary>
    /// Drop an item from the table based on the specified number.
    /// </summary>
    /// <returns>
    /// The item that was dropped.
    /// </returns>
    public ILootItem Drop(
        double number
    )
    {
        var index = _weightTable.SelectIndex(number);

        var itemToDrop = _loot[index];

        return itemToDrop;
    }

    public static Either<IEnumerable<ValidationError>, LootTable<TWeightTable>> Create(
        IReadOnlyList<ILootItem> loot,
        Dictionary<string, double> variables
    )
    {
        var lootTable =
            new LootTable<TWeightTable>();

        return
            lootTable.Initialize(
                loot,
                variables
            ).Match(
                Left<IEnumerable<ValidationError>, LootTable<TWeightTable>>,
                _ => Right<IEnumerable<ValidationError>, LootTable<TWeightTable>>(lootTable)
            );
    }
}