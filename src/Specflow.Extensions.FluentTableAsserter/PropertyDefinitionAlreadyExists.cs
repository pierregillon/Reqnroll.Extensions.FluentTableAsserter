using System;

namespace Specflow.Extensions.FluentTableAsserter;

public class PropertyDefinitionAlreadyExists : Exception
{
    public PropertyDefinitionAlreadyExists(string propertyDefinitionString) : base(
        $"The same property definition exists: {propertyDefinitionString}")
    {
    }
}