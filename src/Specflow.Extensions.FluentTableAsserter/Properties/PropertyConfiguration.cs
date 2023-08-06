using System;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyConfiguration<T, TProperty>(string? ColumnName, Func<string, TProperty>? ColumnValueConversion)
    : ISingleObjectPropertyConfiguration<T, TProperty>, ICollectionPropertyConfiguration<T, TProperty>
{
    public static PropertyConfiguration<T, TProperty> Default => new(null, default);

    ISingleObjectPropertyConfiguration<T, TProperty> ISingleObjectPropertyConfiguration<T, TProperty>.ComparedToField(
        string columnName
    ) =>
        this with { ColumnName = columnName };

    ISingleObjectPropertyConfiguration<T, TProperty> ISingleObjectPropertyConfiguration<T, TProperty>.
        WithFieldValueConversion(Func<string, TProperty> conversion) =>
        this with { ColumnValueConversion = conversion };

    ICollectionPropertyConfiguration<T, TProperty> ICollectionPropertyConfiguration<T, TProperty>.ComparedToColumn(
        string columnName
    ) =>
        this with { ColumnName = columnName };

    ICollectionPropertyConfiguration<T, TProperty> ICollectionPropertyConfiguration<T, TProperty>.
        WithColumnValueConversion(Func<string, TProperty> conversion) =>
        this with { ColumnValueConversion = conversion };
}