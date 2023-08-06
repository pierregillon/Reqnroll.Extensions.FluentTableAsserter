using System;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;

public class DuplicateColumnDefinitionException : Exception
{
    public DuplicateColumnDefinitionException(Type elementType, string[] headers, string propertyName)
        : base(
            $"Columns {string.Join(", ", headers)} are duplicates: they match the same property definition '{propertyName}' of class '{elementType.Name}'.")
    {
    }
}