using MassLoot.Utilities;

namespace MassLoot.Tests;

[TestFixture]
[TestOf(typeof(LootTableTests))]
public class LootTableTests
{
    [Test]
    public void EmptyTableThrowsException()
    {
        var result =
            LootTable<BinaryIndexedWeightTable>.Create(
                new List<LootItem>(),
                new Dictionary<string, double>()
            ).Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed.")
            );

        Assert.That(
            result,
            Has.Exactly(1).Items
        );
    }

    [Test]
    public void TableWithMalformedItemExpressionFails()
    {
        var result =
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("item1", "5 +++ 5")
                ],
                new Dictionary<string, double>()
            ).Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed.")
            );

        Assert.That(
            result,
            Has.Exactly(2).Items
        );
    }

    [Test]
    public void DropOneItem()
    {
        var lootTable =
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("item_1", "1")
                ],
                new Dictionary<string, double>()
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
            );

        var result = lootTable.Drop(0.5d);

        Assert.That(result.ItemId, Is.EqualTo("item_1"));
    }

    [Test]
    public void DropOneItemAfterUpdatingVariable()
    {
        var lootTable =
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("item_1", "1"),
                    new LootItem("item_2", "1 + test_var")
                ],
                new Dictionary<string, double>
                {
                    { "test_var", 0d }
                }
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
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
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("item_1", "1"),
                    new LootItem("item_2", "1")
                ],
                new Dictionary<string, double>()
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
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
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("item_1", "1 + test_var"),
                    new LootItem("item_2", "1"),
                ],
                new Dictionary<string, double>
                {
                    { "test_var", 0d }
                }
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
            );

        var item1 = lootTable.Drop(0.51d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.ItemId, Is.EqualTo("item_1"));
        });
    }

    [Test]
    public void DontDropItemWithZeroWeight()
    {
        var lootTable =
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("item_1", "1"),
                    new LootItem("item_2", "1"),
                    new LootItem("item_3", "0")
                ],
                new Dictionary<string, double>()
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
            );

        var item1 = lootTable.Drop(0d);
        var item2 = lootTable.Drop(0.50000001d);
        var item3 = lootTable.Drop(0.7d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.ItemId, Is.EqualTo("item_1"));
            Assert.That(item2.ItemId, Is.EqualTo("item_2"));
            Assert.That(item3.ItemId, Is.EqualTo("item_2"));
        });
    }

    [Test]
    public void DropItemWithZeroWeightAtBeginningOfTable()
    {
        var lootTable =
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("item_1", "0"),
                    new LootItem("item_2", "1")
                ],
                new Dictionary<string, double>()
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
            );

        var item1 = lootTable.Drop(0d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.ItemId, Is.EqualTo("item_1"));
        });
    }

    [Test]
    public void DontDropItemWithZeroWeightAfterUpdatingVariable()
    {
        var lootTable =
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("w1", "1000 + test_var"),
                    new LootItem("w1", "5")
                ],
                new Dictionary<string, double>
                {
                    { "test_var", 0d }
                }
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
            );

        var item1 = lootTable.Drop(0d);
        var item2 = lootTable.Drop(0.7d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.Weight, Is.EqualTo(5d));
            Assert.That(item2.Weight, Is.EqualTo(1000d));
        });

        lootTable.UpdateVariable("test_var", -1000d);

        var item3 = lootTable.Drop(1d);

        Assert.That(item3.Weight, Is.EqualTo(5d));
    }

    [Test]
    public void ZeroWeightItemIsDroppedAfterBeingUpdated()
    {
        var lootTable =
            LootTable<BinaryIndexedWeightTable>.Create(
                [
                    new LootItem("i0", "magic_find"),
                    new LootItem("i1", "5")
                ],
                new Dictionary<string, double>
                {
                    { "magic_find", 0d }
                }
            ).Match(
                _ => Assert.Fail("Validation failed."),
                right => right
            );

        var item1 = lootTable.Drop(0.000001d);

        Assert.Multiple(() =>
        {
            Assert.That(item1.Weight, Is.EqualTo(5d));
        });

        const double newVarValue = 167d;
        lootTable.UpdateVariable("magic_find", newVarValue);

        var item2 = lootTable.Drop(5.00000001d);

        Assert.That(item2.Weight, Is.EqualTo(newVarValue));
    }
}