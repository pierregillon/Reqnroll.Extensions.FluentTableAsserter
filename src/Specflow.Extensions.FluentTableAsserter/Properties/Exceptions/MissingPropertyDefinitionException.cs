using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

public class MissingPropertyDefinitionException : Exception
{
    public MissingPropertyDefinitionException(Type elementType, string header)
        : base($"Column or field '{header}' has not been mapped to any property of class '{elementType.Name}'.")
    {
    }
}