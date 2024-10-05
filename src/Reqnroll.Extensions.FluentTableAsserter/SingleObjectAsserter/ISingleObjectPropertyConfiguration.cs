using System;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectPropertyConfiguration<in TObject, TProperty>
{
    public ISingleObjectPropertyConfiguration<TObject, TProperty> ComparedToField(string columnName);

    [Obsolete(
        $"Use {nameof(ISingleObjectPropertyConfiguration<object, object>.WithFieldToPropertyConversion)} instead."
    )]
    public ISingleObjectPropertyConfiguration<TObject, TProperty> WithFieldValueConversion(
        Func<string, TProperty> convert
    );

    public ISingleObjectPropertyConfiguration<TObject, TProperty> WithFieldToPropertyConversion(
        Func<string, TProperty> convert
    );

    ISingleObjectPropertyConfiguration<TObject, TTransformedProperty> WithPropertyTransformation<TTransformedProperty>(
        Func<TProperty, TTransformedProperty> transform
    );
}