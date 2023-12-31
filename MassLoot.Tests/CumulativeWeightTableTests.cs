namespace MassLoot.Tests;

[TestFixture]
[TestOf(typeof(CumulativeWeightTableTests))]
public class CumulativeWeightTableTests
{
    [Test]
    public void EmptyTableThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () => new CumulativeWeightTable(
                new()
            )
        );
    }

    [TestCase(0, 0)]
    [TestCase(1, 1)]
    public void DropLegendaryItem(
        double randomNumber,
        int expectedIndex
    )
    {
        List<LootItem> lootItems = [
            new("common_item", "999"),
            new("legendary_item", "1")
        ];

        foreach (var lootItem in lootItems)
        {
            lootItem.Calculate(
                new Dictionary<string, double>()
            );
        }

        var cumulativeWeightTable =
            new CumulativeWeightTable(
                lootItems
            );

        var result = cumulativeWeightTable.SelectIndex(randomNumber);

        Assert.That(result, Is.EqualTo(expectedIndex));
    }

    [Test]
    public void DropTheOnlyItem()
    {
        List<LootItem> lootItems = [
            new("common_item", "1")
        ];

        foreach (var lootItem in lootItems)
        {
            lootItem.Calculate(
                new Dictionary<string, double>()
            );
        }

        var cumulativeWeightTable =
            new CumulativeWeightTable(
                lootItems
            );

        var result = cumulativeWeightTable.SelectIndex(0.5d);

        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void DropWithSmallWeight()
    {
        List<LootItem> lootItems = [
            new("item_1", "0"),
            new("item_2", "1"),
            new("item_3", "1"),
        ];

        foreach (var lootItem in lootItems)
        {
            lootItem.Calculate(
                new Dictionary<string, double>()
            );
        }

        var cumulativeWeightTable =
            new CumulativeWeightTable(
                lootItems
            );

        var result = cumulativeWeightTable.SelectIndex(0.000000024d);

        Assert.That(result, Is.EqualTo(1));
    }
}