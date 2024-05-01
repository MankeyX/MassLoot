using MassLoot.Utilities;
using NUnit.Framework;

namespace MassLoot.Expressions.Tests;

[TestFixture]
[TestOf(typeof(ExpressionTokensExtensions))]
public class ExpressionTokensExtensionsTests
{
    [Test]
    public void ValidateReturnsTrueForValidExpression()
    {
        var expressionTokens =
            new ExpressionTokens(
            [
                new ExpressionToken("1"),
                new ExpressionToken("+"),
                new ExpressionToken("2")
            ])
            .Validate()
            .Match(
                _ => Assert.Fail("Validation failed"),
                right => right
            );

        Assert.That(
            string.Join("", expressionTokens.Select(x => x.Token)),
            Is.EqualTo("1+2")
        );
    }

    [Test]
    public void ValidateReturnsFalseForEmptyExpression()
    {
        var validationErrors =
            new ExpressionTokens([])
                .Validate()
                .Match(
                    left => left,
                    _ => Assert.Fail("Validation passed when it should have failed")
                );

        Assert.That(
            validationErrors,
            Has.One.Items.Matches<ValidationError>(
                x => x.Type == ValidationErrorType.EmptyExpression
            )
        );
    }

    [Test]
    public void ValidateReturnsFalseForExpressionBeginningWithOperator()
    {
        var validationErrors =
            new ExpressionTokens(
                [
                    new ExpressionToken("+"),
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("2")
                ]
            )
            .Validate()
            .Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed")
            );

        Assert.That(
            validationErrors,
            Has.One.Items.Matches<ValidationError>(
                x => x.Type == ValidationErrorType.MalformedExpression
            )
        );
    }

    [Test]
    public void ValidateReturnsFalseForExpressionEndingWithOperator()
    {
        var validationErrors =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("2"),
                    new ExpressionToken("+")
                ]
            )
            .Validate()
            .Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed")
            );

        Assert.That(
            validationErrors,
            Has.One.Items.Matches<ValidationError>(
                x => x.Type == ValidationErrorType.MalformedExpression
            )
        );
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithClosingParenthesisBeforeOpeningParenthesis()
    {
        var validationErrors =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken(")"),
                    new ExpressionToken("("),
                    new ExpressionToken("2")
                ]
            )
            .Validate()
            .Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed")
            );

        Assert.That(
            validationErrors,
            Has.One.Items.Matches<ValidationError>(
                x => x.Type == ValidationErrorType.MalformedExpression
            )
        );
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithMismatchedParenthesis()
    {
        var validationErrors =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("("),
                    new ExpressionToken("2"),
                    new ExpressionToken(")"),
                    new ExpressionToken(")")
                ]
            )
            .Validate()
            .Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed")
            );

        Assert.That(
            validationErrors,
            Has.Exactly(2).Items.Matches<ValidationError>(
                x => x.Type == ValidationErrorType.MalformedExpression
            )
        );
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithConsecutiveOperators()
    {
        var validationErrors =
            new ExpressionTokens(
                [
                    new ExpressionToken("1"),
                    new ExpressionToken("+"),
                    new ExpressionToken("+"),
                    new ExpressionToken("2")
                ]
            )
            .Validate()
            .Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed")
            );

        Assert.That(
            validationErrors,
            Has.One.Items.Matches<ValidationError>(
                x => x.Type == ValidationErrorType.MalformedExpression
            )
        );
    }

    [Test]
    public void ValidateReturnsFalseForExpressionWithMultipleErrors()
    {
        var validationErrors =
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
            )
            .Validate()
            .Match(
                left => left,
                _ => Assert.Fail("Validation passed when it should have failed")
            );

        Assert.That(
            validationErrors,
            Has.Exactly(5).Items.Matches<ValidationError>(
                x => x.Type == ValidationErrorType.MalformedExpression
            )
        );
    }
}