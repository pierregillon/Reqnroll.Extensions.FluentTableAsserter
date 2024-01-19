using System;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyConfiguration<T, TProperty>(
    string? ColumnName,
    Func<string, TProperty>? ColumnToPropertyConversion,
    Func<TProperty, object>? PropertyToColumnConversion
) : ISingleObjectPropertyConfiguration<T, TProperty>, ICollectionPropertyConfiguration<T, TProperty>
{
    public static PropertyConfiguration<T, TProperty> Default => new(null, default, default);

    ISingleObjectPropertyConfiguration<T, TProperty> ISingleObjectPropertyConfiguration<T, TProperty>.ComparedToField(
        string columnName
    ) =>
        this with { ColumnName = columnName };

    ISingleObjectPropertyConfiguration<T, TProperty> ISingleObjectPropertyConfiguration<T, TProperty>.
        WithFieldValueConversion(Func<string, TProperty> conversion) =>
        this with { ColumnToPropertyConversion = conversion };

    public ISingleObjectPropertyConfiguration<T, TProperty> WithFieldToPropertyConversion(
        Func<string, TProperty> conversion
    ) =>
        this with { ColumnToPropertyConversion = conversion };

    public ISingleObjectPropertyConfiguration<T, TProperty> WithPropertyToFieldConversion<TNewProperty>(
        Func<TProperty, TNewProperty> func
    )
        where TNewProperty : notnull =>
        this with { PropertyToColumnConversion = p => func(p) };

    ICollectionPropertyConfiguration<T, TProperty> ICollectionPropertyConfiguration<T, TProperty>.ComparedToColumn(
        string columnName
    ) =>
        this with { ColumnName = columnName };

    ICollectionPropertyConfiguration<T, TProperty> ICollectionPropertyConfiguration<T, TProperty>.
        WithColumnValueConversion(Func<string, TProperty> conversion) =>
        this with { ColumnToPropertyConversion = conversion };
}