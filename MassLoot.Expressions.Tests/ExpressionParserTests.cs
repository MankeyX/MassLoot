using NUnit.Framework;

namespace MassLoot.Expressions.Tests;

public class ExpressionParserTests
{
    [Test]
    public void ParseComplex()
    {
        const string expressionToParse = "3 ^ 2 * 4 + ( 5 - 6 )";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.Multiple(() =>
        {
            Assert.That(expression, Is.Not.Null);
            Assert.That(
                string.Join(null, expression.Tokens.Select(x => x.Token)),
                Is.EqualTo("32^4*56-+")
            );
        });
    }

    [Test]
    public void ParseComplexWithVariables()
    {
        const string expressionToParse = "t0_modifier + 3 ^ 2 * 4 + (5 - 6)";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.Multiple(() =>
        {
            Assert.That(expression, Is.Not.Null);
            Assert.That(
                string.Join(null, expression.Tokens.Select(x => x.Token)),
                Is.EqualTo("t0_modifier32^4*56-++")
            );
        });
    }

    [Test]
    public void ParseComplexWithVariablesAndPercent()
    {
        const string expressionToParse = "0.1% * (magic_find / 100 + 1)";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.Multiple(() =>
        {
            Assert.That(expression, Is.Not.Null);
            Assert.That(
                string.Join(null, expression.Tokens.Select(x => x.Token)),
                Is.EqualTo("0.1%magic_find100/1+*")
            );
        });
    }

    [Test]
    public void ParseAddition()
    {
        const string expressionToParse = "5 + 3";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            string.Join(null, expression.Tokens.Select(x => x.Token)),
            Is.EqualTo("53+")
        );
    }

    [Test]
    public void ParseSubtraction()
    {
        const string expressionToParse = "5 - 3";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            string.Join(null, expression.Tokens.Select(x => x.Token)),
            Is.EqualTo("53-")
        );
    }

    [Test]
    public void ParseNoOperation()
    {
        const string expressionToParse = "5";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            string.Join(null, expression.Tokens.Select(x => x.Token)),
            Is.EqualTo("5")
        );
    }

    [Test]
    public void ParseMultipleAddition()
    {
        const string expressionToParse = "5 + 3 + 2";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            string.Join(null, expression.Tokens.Select(x => x.Token)),
            Is.EqualTo("53+2+")
        );
    }

    [Test]
    public void ParseMixedOperations()
    {
        const string expressionToParse = "5 + 3 * 2";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            string.Join(null, expression.Tokens.Select(x => x.Token)),
            Is.EqualTo("532*+")
        );
    }

    [Test]
    public void ParseNestedParentheses()
    {
        const string expressionToParse = "5 + ( 3 - ( 2 + 1 ) )";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            string.Join(null, expression.Tokens.Select(x => x.Token)),
            Is.EqualTo("5321+-+")
        );
    }

    [Test]
    public void ParseMultipleParentheses()
    {
        const string expressionToParse = "( 5 + 3 ) * ( 2 - 1 )";
        var expression = ExpressionParser.Parse(expressionToParse);

        Assert.That(
            string.Join(null, expression.Tokens.Select(x => x.Token)),
            Is.EqualTo("53+21-*")
        );
    }

    [Test]
    public void ParseMismatchedParentheses()
    {
        const string expressionToParse = "5 + ( 3 - 2";

        Assert.Throws<InvalidOperationException>(
            () => ExpressionParser.Parse(expressionToParse)
        );
    }

    [Test]
    public void ParseConsecutiveOperators()
    {
        const string expressionToParse = "5 + + 3";

        Assert.Throws<InvalidOperationException>(
            () => ExpressionParser.Parse(expressionToParse)
        );
    }

    [Test]
    public void ParseStartingWithOperator()
    {
        const string expressionToParse = "+ 5 + 3";

        Assert.Throws<InvalidOperationException>(
            () => ExpressionParser.Parse(expressionToParse)
        );
    }

    [Test]
    public void ParseEndingWithOperator()
    {
        const string expressionToParse = "5 + 3 +";

        Assert.Throws<InvalidOperationException>(
            () => ExpressionParser.Parse(expressionToParse)
        );
    }
}