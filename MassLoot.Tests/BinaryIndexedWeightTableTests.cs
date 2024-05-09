namespace MassLoot.Tests;

[TestFixture]
[TestOf(typeof(IWeightTable))]
public class BinaryIndexedWeightTableTests
{
    private static readonly List<LootItem> LootItems =
    [
        new("item_1", "1"),
        new("item_2", "0"),
        new("item_3", "0"),
        new("item_4", "500")
    ];

    [SetUp]
    public void InitializeItems()
    {
        foreach (var item in LootItems)
        {
            item.Initialize(new Dictionary<string, double>());
        }
    }

    [Test]
    public void SelectIndex_DropItemThatHasWeight()
    {
        var table = new BinaryIndexedWeightTable();
        table.Initialize(LootItems);

        var index = table.SelectIndex(0.1d);
        var droppedItem = LootItems[index];

        Assert.Multiple(() =>
        {
            Assert.That(index, Is.EqualTo(0).Or.EqualTo(3));
            Assert.That(droppedItem.Weight, Is.GreaterThan(0));
        });
    }

    [Test]
    public void SelectIndex_SelectsNegativeIndex()
    {
        var table = new BinaryIndexedWeightTable();
        table.Initialize(Array.Empty<ILootItem>());

        var index = table.SelectIndex(0.1d);

        Assert.That(index, Is.EqualTo(-1));
    }
}