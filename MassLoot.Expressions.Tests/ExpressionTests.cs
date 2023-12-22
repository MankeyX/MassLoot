using NUnit.Framework;

namespace MassLoot.Expressions.Tests;

public class ExpressionTests
{
    [TestCase("5*(3+2)-1", 24)]
    [TestCase("( 5 + 3 ) * ( 2 + 1 )", 24)]
    [TestCase("(5 + 3) * (2 + 1) + -5", 19)]
    [TestCase("3 + 3 ^ 2 * 4 + (5 - 6)", 38)]
    [TestCase("5 + 3 * 2 / 2", 8)]
    [TestCase("5 + 3 * 2 / 2 - 1", 7)]
    [TestCase("5 / 0", double.PositiveInfinity)]
    [TestCase("5.5 + 3.5", 9)]
    [TestCase("-5.5 + 3.5", -2)]
    public void CalculateComplex(
        string expressionToParse,
        double expectedResult
    )
    {
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            expression.Calculate(),
            Is.EqualTo(expectedResult)
        );
    }

    [TestCase("4+4", 8)]
    [TestCase("4 + 4", 8)]
    [TestCase("4 / 4", 1)]
    [TestCase("4 * 4", 16)]
    [TestCase("4 - 4", 0)]
    [TestCase("4 + 4 + 4 + 4 + 4", 20)]
    [TestCase("4 / 4 / 4 / 4 / 4", 0.015625d)]
    public void CalculateSimple(
        string expressionToParse,
        double expectedResult
    )
    {
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            expression.Calculate(),
            Is.EqualTo(expectedResult)
        );
    }
}