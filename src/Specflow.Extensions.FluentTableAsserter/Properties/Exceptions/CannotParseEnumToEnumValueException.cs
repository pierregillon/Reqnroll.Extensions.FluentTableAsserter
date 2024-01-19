using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

internal class CannotParseEnumToEnumValueException(string invalidEnumValue, Type type)
    : Exception($"'{invalidEnumValue}' cannot be parsed to any enum value of type {type.Name}.");