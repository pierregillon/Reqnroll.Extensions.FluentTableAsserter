using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

public class PropertyDefinitionAlreadyExistsException(string propertyDefinitionString)
    : Exception($"The same property definition exists: {propertyDefinitionString}");