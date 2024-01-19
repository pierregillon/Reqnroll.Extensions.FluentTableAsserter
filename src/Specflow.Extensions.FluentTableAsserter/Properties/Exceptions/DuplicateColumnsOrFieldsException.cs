using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

public class DuplicateColumnsOrFieldsException(Type elementType, string[] headers, string propertyName) : Exception(
    $"Columns or fields {string.Join(", ", headers)} are duplicates: they match the same property definition '{propertyName}' of class '{elementType.Name}'.");