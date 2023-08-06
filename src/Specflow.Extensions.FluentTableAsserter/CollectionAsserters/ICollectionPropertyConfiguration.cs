using System;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public interface ICollectionPropertyConfiguration<T, in TProperty>
{
    public ICollectionPropertyConfiguration<T, TProperty> ComparedToColumn(string columnName);

    public ICollectionPropertyConfiguration<T, TProperty> WithColumnValueConversion(
        Func<string, TProperty> conversion
    );
}