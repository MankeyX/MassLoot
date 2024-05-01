namespace MassLoot.Utilities;

public record ValidationError(
    ValidationErrorType Type,
    string Message
);

public enum ValidationErrorType
{
    EmptyExpression,
    MalformedExpression,
    EmptyTable
}