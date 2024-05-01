namespace MassLoot.Tests;

[TestFixture]
public class EqualityTests
{
    [Test]
    public void TestItemId()
    {
        var item1 = new LootItem("item_1", "1");
        var item2 = new LootItem("item_1", "321");

        Assert.That(item1, Is.EqualTo(item2));
    }

    [Test]
    public void TestNull()
    {
        var item1 = new LootItem("item_1", "1");
        LootItem item2 = null!;

        Assert.That(item1, Is.Not.EqualTo(item2));
    }

    [Test]
    public void TestType_NotEqual()
    {
        var item1 = new LootItem("item_1", "1");
        var item2 = new object();

        Assert.That(item2, Is.Not.EqualTo(item1));
    }

    [Test]
    public void TestType_Equal()
    {
        var item1 = new LootItem("item_1", "1");
        var item2 = new LootItem("item_1", "321");

        Assert.That((object)item1, Is.EqualTo(item2));
    }
}