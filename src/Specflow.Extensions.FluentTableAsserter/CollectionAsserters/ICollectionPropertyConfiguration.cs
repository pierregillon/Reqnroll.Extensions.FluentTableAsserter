using System;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public interface ICollectionPropertyConfiguration<TCollection, TProperty>
{
    public ICollectionPropertyConfiguration<TCollection, TProperty> ComparedToColumn(string columnName);

    [Obsolete(
        $"Use {nameof(ICollectionPropertyConfiguration<object, object>.WithColumnToPropertyConversion)} instead."
    )]
    public ICollectionPropertyConfiguration<TCollection, TProperty> WithColumnValueConversion(
        Func<string, TProperty> convert
    );

    public ICollectionPropertyConfiguration<TCollection, TProperty> WithColumnToPropertyConversion(
        Func<string, TProperty> convert
    );

    ICollectionPropertyConfiguration<TCollection, TTransformedProperty>
        WithPropertyTransformation<TTransformedProperty>(
            Func<TProperty, TTransformedProperty> transform
        );
}