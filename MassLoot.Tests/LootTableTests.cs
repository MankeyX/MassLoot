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
                new(),
                new()
            )
        );
    }

    [Test]
    public void DropOneItem()
    {
        var lootTable =
            new LootTable(
                [
                    new("item_1", "1")
                ],
                new()
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
                    new("item_1", "1"),
                    new("item_2", "1 + test_var")
                ],
                new()
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
                    new("item_1", "1"),
                    new("item_2", "1")
                ],
                new()
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
                    new("item_1", "1 + test_var"),
                    new("item_2", "1"),
                ],
                new()
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
}