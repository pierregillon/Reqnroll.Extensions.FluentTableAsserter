using System;
using System.Reflection;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;

internal class CannotConvertCellValueToPropertyTypeException : Exception
{
    public CannotConvertCellValueToPropertyTypeException(
        string value,
        string propertyName,
        MemberInfo propertyType,
        MemberInfo elementType,
        Exception innerException
    )
        : base(
            $"The value '{value}' cannot be converted to type '{propertyType.Name}' of property '{elementType.Name}.{propertyName}'",
            innerException)
    {
    }
}