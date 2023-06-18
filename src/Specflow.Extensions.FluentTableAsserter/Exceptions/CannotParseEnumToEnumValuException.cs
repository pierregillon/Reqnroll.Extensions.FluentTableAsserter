using System;

namespace Specflow.Extensions.FluentTableAsserter.Exceptions;

internal class CannotParseEnumToEnumValuException<T> : Exception
{
    public CannotParseEnumToEnumValuException(string invalidEnumValue)
        : base($"'{invalidEnumValue}' cannot be parsed to any enum value of type {typeof(T).Name}.")
    {
    }
}