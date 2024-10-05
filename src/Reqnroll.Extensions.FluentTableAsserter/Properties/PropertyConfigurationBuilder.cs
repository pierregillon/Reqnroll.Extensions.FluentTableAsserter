using System;
using Reqnroll.Extensions.FluentTableAsserter.CollectionAsserters;
using Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter;

// ReSharper disable WithExpressionModifiesAllMembers

namespace Reqnroll.Extensions.FluentTableAsserter.Properties;

public record PropertyConfigurationBuilder<T, TProperty>(
    PropertyConfiguration Value
) :
    ISingleObjectPropertyConfiguration<T, TProperty>,
    ICollectionPropertyConfiguration<T, TProperty>
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
        WithFieldValueConversion(Func<string, TProperty> convert) =>
        this with
        {
            Value = Value with
            {
                ColumnToPropertyConversion = s => convert(s)
            }
        };

    public ISingleObjectPropertyConfiguration<T, TProperty> WithFieldToPropertyConversion(
        Func<string, TProperty> convert
    ) =>
        this with
        {
            Value = Value with
            {
                ColumnToPropertyConversion = s => convert(s)
            }
        };

    public ISingleObjectPropertyConfiguration<T, TNewProperty> WithPropertyTransformation<TNewProperty>(
        Func<TProperty, TNewProperty> transform
    ) =>
        new PropertyConfigurationBuilder<T, TNewProperty>(Value with
        {
            PropertyConversion = p => transform((TProperty)p!)
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

    public ICollectionPropertyConfiguration<T, TProperty>
        WithColumnValueConversion(Func<string, TProperty> convert) =>
        this with
        {
            Value = Value with
            {
                ColumnToPropertyConversion = s => convert(s)
            }
        };

    ICollectionPropertyConfiguration<T, TProperty> ICollectionPropertyConfiguration<T, TProperty>.
        WithCellToPropertyConversion(Func<string, TProperty> convert) =>
        this with
        {
            Value = Value with
            {
                ColumnToPropertyConversion = s => convert(s)
            }
        };

    ICollectionPropertyConfiguration<T, TNewProperty> ICollectionPropertyConfiguration<T, TProperty>.
        WithPropertyTransformation<TNewProperty>(Func<TProperty, TNewProperty> transform) =>
        new PropertyConfigurationBuilder<T, TNewProperty>(Value with
        {
            PropertyConversion = p => transform((TProperty)p!)
        });
}