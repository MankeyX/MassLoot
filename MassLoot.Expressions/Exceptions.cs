namespace MassLoot.Expressions;

public class NotEnoughOperandsException(string message) : Exception(message);
public class VariableNotDefinedException(string message) : Exception(message);
public class MalformedExpressionException(string message) : Exception(message);

public class InvalidExpressionException(
    string message,
    ValidationError[] Errors
) : Exception(message);