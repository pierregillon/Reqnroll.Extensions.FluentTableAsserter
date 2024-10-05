using System;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectPropertyConfiguration<in TObject, TProperty>
{
    public ISingleObjectPropertyConfiguration<TObject, TProperty> ComparedToField(string columnName);

    public ISingleObjectPropertyConfiguration<TObject, TProperty> WithFieldToPropertyConversion(
        Func<string, TProperty> convert
    );

    ISingleObjectPropertyConfiguration<TObject, TTransformedProperty> WithPropertyTransformation<TTransformedProperty>(
        Func<TProperty, TTransformedProperty> transform
    );

    ISingleObjectPropertyConfiguration<TObject, TProperty> NonStrictEnumerableComparison();
}