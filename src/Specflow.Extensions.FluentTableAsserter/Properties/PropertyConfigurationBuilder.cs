using System;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

// ReSharper disable WithExpressionModifiesAllMembers

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyConfigurationBuilder<T, TProperty>(
    PropertyConfiguration Value
) : ISingleObjectPropertyConfiguration<T, TProperty>, ICollectionPropertyConfiguration<T, TProperty>
{
    public static PropertyConfigurationBuilder<T, TProperty> Default => new(PropertyConfiguration.Default);

    ISingleObjectPropertyConfiguration<T, TProperty> ISingleObjectPropertyConfiguration<T, TProperty>.ComparedToField(
        string columnName
    ) =>
        this with
        {
            Value = Value with
            {
                ColumnName = columnName
            }
        };

    ISingleObjectPropertyConfiguration<T, TProperty> ISingleObjectPropertyConfiguration<T, TProperty>.
        WithFieldValueConversion(Func<string, TProperty> conversion) =>
        this with
        {
            Value = Value with
            {
                ColumnToPropertyConversion = s => s
            }
        };

    public ISingleObjectPropertyConfiguration<T, TProperty> WithFieldToPropertyConversion(
        Func<string, TProperty> conversion
    ) =>
        this with
        {
            Value = Value with
            {
                ColumnToPropertyConversion = s => conversion(s)
            }
        };

    public ISingleObjectPropertyConfiguration<T, TNewProperty> WithPropertyTransformation<TNewProperty>(
        Func<TProperty, TNewProperty> conversion
    ) =>
        new PropertyConfigurationBuilder<T, TNewProperty>(Value with
        {
            PropertyConversion = p => conversion((TProperty)p!)
        });

    ICollectionPropertyConfiguration<T, TProperty> ICollectionPropertyConfiguration<T, TProperty>.ComparedToColumn(
        string columnName
    ) =>
        this with
        {
            Value = Value with
            {
                ColumnName = columnName
            }
        };

    ICollectionPropertyConfiguration<T, TProperty> ICollectionPropertyConfiguration<T, TProperty>.
        WithColumnValueConversion(Func<string, TProperty> conversion) =>
        this with
        {
            Value = Value with
            {
                ColumnToPropertyConversion = s => conversion(s)
            }
        };
}