using MassLoot;

namespace MassLoot.Tests;

[TestFixture]
[TestOf(typeof(IWeightTable))]
public class SharedWeightTableTests
{
    private static readonly List<LootItem> LootItems =
    [
        new("item_1", "1"),
        new("item_2", "0"),
        new("item_3", "0"),
        new("item_4", "500")
    ];

    private static IEnumerable<IWeightTable> TableSource()
    {
        foreach (var item in LootItems)
        {
            item.Calculate(
                new Dictionary<string, double>()
            );
        }

        var biwt = new BinaryIndexedWeightTable();
        biwt.Initialize(LootItems);

        yield return biwt;

        var awt = new AliasWeightTable();
        awt.Initialize(LootItems);

        yield return awt;
    }

    [TestCaseSource(nameof(TableSource))]
    public void SelectIndex_DropItemThatHasWeight(
        IWeightTable table
    )
    {
        var index = table.SelectIndex(0.1d);
        var item = LootItems[index];

        Assert.Multiple(() =>
        {
            Assert.That(index, Is.EqualTo(0).Or.EqualTo(3));
            Assert.That(item.Weight, Is.GreaterThan(0));
        });
    }
}