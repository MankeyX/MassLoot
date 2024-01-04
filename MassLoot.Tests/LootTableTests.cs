namespace MassLoot.Tests;

[TestFixture]
[TestOf(typeof(LootTableTests))]
public class LootTableTests
{
    [Test]
    public void EmptyTableThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () => new LootTable(
                new List<LootItem>(),
                new Dictionary<string, double>()
            )
        );
    }

    [Test]
    public void DropOneItem()
    {
        var lootTable =
            new LootTable(
                [
                    new LootItem("item_1", "1")
                ],
                new Dictionary<string, double>()
            );

        var result = lootTable.Drop(0.5d);

        Assert.That(result.ItemId, Is.EqualTo("item_1"));
    }

    [Test]
    public void DropOneItemAfterUpdatingVariable()
    {
        var lootTable =
            new LootTable(
                [
                    new LootItem("item_1", "1"),
                    new LootItem("item_2", "1 + test_var")
                ],
                new Dictionary<string, double>
                {
                    { "test_var", 0d }
                }
            );

        var item1 = lootTable.Drop(0.5d);

        lootTable.UpdateVariable("test_var", 1d);
        var item2 = lootTable.Drop(0.5d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.ItemId, Is.EqualTo("item_1"));
            Assert.That(item2.ItemId, Is.EqualTo("item_2"));
        });
    }

    [Test]
    public void DropSameItemAfterUpdatingMissingVariable()
    {
        var lootTable =
            new LootTable(
                [
                    new LootItem("item_1", "1"),
                    new LootItem("item_2", "1")
                ],
                new Dictionary<string, double>()
            );

        var item1 = lootTable.Drop(0.5d);

        lootTable.UpdateVariable("test_var", 1d);
        var item2 = lootTable.Drop(0.5d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.ItemId, Is.EqualTo("item_1"));
            Assert.That(item2.ItemId, Is.EqualTo("item_1"));
        });
    }

    [Test]
    public void DropSortedItemThatUsesVariables()
    {
        var lootTable =
            new LootTable(
                [
                    new LootItem("item_1", "1 + test_var"),
                    new LootItem("item_2", "1"),
                ],
                new Dictionary<string, double>
                {
                    { "test_var", 0d }
                }
            );

        var item1 = lootTable.Drop(0.51d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.ItemId, Is.EqualTo("item_1"));
        });
    }

    [Test]
    public void EmptyTableAsLootItemThrowsException()
    {
        Assert.Throws<ArgumentException>(
            () => new LootTable(
                string.Empty,
                "4+4",
                new List<LootItem>(),
                new Dictionary<string, double>()
            )
        );
    }

    [Test]
    public void DropItemFromNestedLootTable()
    {
        var lootTable =
            new LootTable(
                [
                    new LootItem("item_1", "1"),
                    new LootTable(
                        "table_2",
                        "1",
                        [
                            new LootItem("item_2", "1"),
                            new LootItem("item_3", "1")
                        ],
                        new Dictionary<string, double>()
                    )
                ],
                new Dictionary<string, double>()
            );

        var item1 = lootTable.Drop(0.49d);
        var item2 = lootTable.Drop(0.51d);
        var item3 = lootTable.Drop(0.76d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.ItemId, Is.EqualTo("item_1"));
            Assert.That(item2.ItemId, Is.EqualTo("item_2"));
            Assert.That(item3.ItemId, Is.EqualTo("item_3"));
        });
    }
}