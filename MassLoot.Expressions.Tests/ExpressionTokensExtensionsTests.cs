using NUnit.Framework;

namespace MassLoot.Expressions.Tests;

[TestFixture]
[TestOf(typeof(ExpressionTokensExtensions))]
public class ExpressionTokensExtensionsTests
{
    [Test]
    public void ValidateReturnsTrueForValidExpression()
    {
        var tokens =
            new ExpressionTokens(
            [
                new ExpressionToken("1"),
                new ExpressionToken("+"),
                new ExpressionToken("2")
            ]);
        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.True
            );
            Assert.That(
                errors,
                Is.Empty
            );
        });
    }

    [Test]
    public void ValidateReturnsFalseForEmptyExpression()
    {
        var tokens =
            new ExpressionTokens(
                []
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.False
            );
            Assert.That(
                errors,
                Is.Not.Empty.With.One.Items.Matches<ValidationError>(
                    x => x.Message.Contains("An expression cannot be empty")
                )
            );
        });
    }

    [Test]
    public void ValidateReturnsFalseForExpressionBeginningWithOperator()
    {
        var tokens =
            new ExpressionTokens(
                [
                    new ExpressionToken("+"),
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("2")
                ]
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.False
            );
            Assert.That(
                errors,
                Is.Not.Empty.With.One.Items.Matches<ValidationError>(
                    x => x.Message.Contains("An expression cannot begin with an operator")
                )
            );
        });
    }

    [Test]
    public void ValidateReturnsFalseForExpressionEndingWithOperator()
    {
        var tokens =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("2"),
                    new ExpressionToken("+")
                ]
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.False
            );
            Assert.That(
                errors,
                Is.Not.Empty.With.One.Items.Matches<ValidationError>(
                    x => x.Message.Contains("An expression cannot end with an operator")
                )
            );
        });
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithClosingParenthesisBeforeOpeningParenthesis()
    {
        var tokens =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken(")"),
                    new ExpressionToken("("),
                    new ExpressionToken("2")
                ]
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.False
            );
            Assert.That(
                errors,
                Is.Not.Empty.With.One.Items.Matches<ValidationError>(
                    x => x.Message.Contains("Closing parenthesis cannot be used before opening parenthesis")
                )
            );
        });
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithMismatchedParenthesis()
    {
        var tokens =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("("),
                    new ExpressionToken("2"),
                    new ExpressionToken(")"),
                    new ExpressionToken(")")
                ]
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.False
            );
            Assert.That(
                errors,
                Is.Not.Empty.With.One.Items.Matches<ValidationError>(
                    x => x.Message.Contains("Mismatched parenthesis")
                )
            );
        });
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithConsecutiveOperators()
    {
        var tokens =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("+"),
                    new ExpressionToken("2")
                ]
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.False
            );
            Assert.That(
                errors,
                Is.Not.Empty.With.One.Items.Matches<ValidationError>(
                    x => x.Message.Contains("Consecutive operators")
                )
            );
        });
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithMultipleErrors()
    {
        var tokens =
            new ExpressionTokens(
                [
                    new ExpressionToken("+"),
                    new ExpressionToken("+"),
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken(")"),
                    new ExpressionToken("("),
                    new ExpressionToken("2"),
                    new ExpressionToken("+"),
                    new ExpressionToken("(")
                ]
            );

        Assert.Multiple(() =>
        {
            Assert.That(
                tokens.Validate(out var errors),
                Is.False
            );
            Assert.That(
                errors,
                Is.Not.Empty.With.Exactly(5).Items
            );
        });
    }
}