using System;
using System.Reflection;

namespace Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

internal class CannotConvertCellValueToPropertyTypeException(
    string value,
    string propertyName,
    MemberInfo propertyType,
    MemberInfo elementType,
    Exception innerException
)
    : Exception(
        $"The value '{value}' cannot be converted to type '{propertyType.Name}' of property '{elementType.Name}.{propertyName}'",
        innerException);