using System;
using System.Reflection;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

namespace Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

internal class CannotConvertCellValueToPropertyTypeException(
    string value,
    string propertyName,
    MemberInfo propertyType,
    MemberInfo elementType,
    Exception innerException
)
    : Exception(
        @$"The value '{value}' cannot be converted to type '{propertyType.Name}' of property '{elementType.Name}.{propertyName}'.
Verify your types or consider using conversion methods to adapt the actual type to the expected type.

For single object property configuration:

singleObject
    .{nameof(ObjectExtensions.ObjectShouldBeEquivalentToTable)}(table)
    .WithProperty(
        x => x.Property,
        x => x
         this // .{nameof(ISingleObjectPropertyConfiguration<object, object>.WithFieldToPropertyConversion)}(propertyValue => ...)
      or this // .{nameof(ISingleObjectPropertyConfiguration<object, object>.WithPropertyTransformation)}(fieldValue => ...)
    )
    .Assert();

For collection property configuration:

singleObject
    .{nameof(EnumerableExtensions.CollectionShouldBeEquivalentToTable)}(table)
    .WithProperty(
        x => x.Property,
        x => x
         this // .{nameof(ICollectionPropertyConfiguration<object, object>.WithCellToPropertyConversion)}(propertyValue => ...)
      or this // .{nameof(ICollectionPropertyConfiguration<object, object>.WithCellToPropertyConversion)}(fieldValue => ...)
    )
    .Assert();

",
        innerException
    );