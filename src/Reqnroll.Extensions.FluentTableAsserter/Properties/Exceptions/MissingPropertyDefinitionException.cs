using System;

namespace Reqnroll.Extensions.FluentTableAsserter.Properties.Exceptions;

public class MissingPropertyDefinitionException(Type elementType, string header)
    : Exception($"Column or field '{header}' has not been mapped to any property of class '{elementType.Name}'.");