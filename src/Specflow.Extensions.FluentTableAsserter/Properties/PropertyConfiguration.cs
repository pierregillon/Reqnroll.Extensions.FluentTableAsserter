using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyConfiguration<T, TProperty>(string? ColumnName, Func<string, TProperty>? ColumnValueConversion)
{
    public static PropertyConfiguration<T, TProperty> Default => new(null, default);

    [Obsolete("It has been renamed. Use 'ComparedToColumn()' instead.")]
    public PropertyConfiguration<T, TProperty> MappedToColumn(string columnName) =>
        this with { ColumnName = columnName };

    public PropertyConfiguration<T, TProperty> ComparedToColumn(string columnName) =>
        this with { ColumnName = columnName };

    [Obsolete("It has been renamed. Use 'WithColumnValueConversion()' instead.")]
    public PropertyConfiguration<T, TProperty> WithColumnConversion(Func<string, TProperty> conversion) =>
        this with { ColumnValueConversion = conversion };

    public PropertyConfiguration<T, TProperty> WithColumnValueConversion(Func<string, TProperty> conversion) =>
        this with { ColumnValueConversion = conversion };
}