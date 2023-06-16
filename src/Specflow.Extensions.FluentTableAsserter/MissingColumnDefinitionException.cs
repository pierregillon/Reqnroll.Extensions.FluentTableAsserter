using System;

namespace Specflow.Extensions.FluentTableAsserter;

public class MissingColumnDefinitionException : Exception
{
    public MissingColumnDefinitionException(Type elementType, string header)
        : base($"The column '{header}' has not been mapped to any property of class '{elementType.Name}'.")
    {
    }
}