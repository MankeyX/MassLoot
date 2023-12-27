namespace MassLoot.Tests;

[TestFixture]
[TestOf(typeof(CumulativeWeightTableTests))]
public class CumulativeWeightTableTests
{
    [TestCase(0, "common_item")]
    [TestCase(1, "legendary_item")]
    public void DropLegendaryItem(
        double randomNumber,
        string expectedItemId
    )
    {
        var cumulativeWeightTable = new CumulativeWeightTable(
        [
            new("common_item", "999"),
            new("legendary_item", "1")
        ]);

        var result = cumulativeWeightTable.Drop(randomNumber);

        Assert.That(result.ItemId, Is.EqualTo(expectedItemId));
    }

    [Test]
    public void DropTheOnlyItem()
    {
        var cumulativeWeightTable = new CumulativeWeightTable(
        [
            new("common_item", "1")
        ]);

        var result = cumulativeWeightTable.Drop(0.5d);

        Assert.That(result.ItemId, Is.EqualTo("common_item"));
    }

    [Test]
    public void DropWithSmallWeight()
    {
        var cumulativeWeightTable = new CumulativeWeightTable(
        [
            new("item_1", "0"),
            new("item_2", "1"),
            new("item_3", "1"),
        ]);

        var result = cumulativeWeightTable.Drop(0.000000024d);

        Assert.That(result.ItemId, Is.EqualTo("item_2"));
    }
}