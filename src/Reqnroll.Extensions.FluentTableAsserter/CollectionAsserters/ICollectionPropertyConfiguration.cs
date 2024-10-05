using System;

namespace Reqnroll.Extensions.FluentTableAsserter.CollectionAsserters;

public interface ICollectionPropertyConfiguration<TCollection, TProperty>
{
    public ICollectionPropertyConfiguration<TCollection, TProperty> ComparedToColumn(string columnName);

    [Obsolete(
        $"Use {nameof(ICollectionPropertyConfiguration<object, object>.WithCellToPropertyConversion)} instead."
    )]
    public ICollectionPropertyConfiguration<TCollection, TProperty> WithColumnValueConversion(
        Func<string, TProperty> convert
    );

    public ICollectionPropertyConfiguration<TCollection, TProperty> WithCellToPropertyConversion(
        Func<string, TProperty> convert
    );

    ICollectionPropertyConfiguration<TCollection, TTransformedProperty>
        WithPropertyTransformation<TTransformedProperty>(
            Func<TProperty, TTransformedProperty> transform
        );
}