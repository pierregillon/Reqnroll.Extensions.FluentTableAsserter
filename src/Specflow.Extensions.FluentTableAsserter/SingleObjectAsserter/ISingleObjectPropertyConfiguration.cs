using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectPropertyConfiguration<in TField, TProperty>
{
    public ISingleObjectPropertyConfiguration<TField, TProperty> ComparedToField(string columnName);

    [Obsolete(
        $"Use {nameof(ISingleObjectPropertyConfiguration<object, object>.WithFieldToPropertyConversion)} instead.")]
    public ISingleObjectPropertyConfiguration<TField, TProperty> WithFieldValueConversion(
        Func<string, TProperty> conversion
    );

    public ISingleObjectPropertyConfiguration<TField, TProperty> WithFieldToPropertyConversion(
        Func<string, TProperty> conversion
    );

    ISingleObjectPropertyConfiguration<TField, TNewProperty> WithPropertyTransformation<TNewProperty>(
        Func<TProperty, TNewProperty> conversion
    );
}