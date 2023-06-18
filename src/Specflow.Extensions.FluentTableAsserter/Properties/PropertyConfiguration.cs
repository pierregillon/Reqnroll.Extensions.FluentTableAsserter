using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyConfiguration<T, TProperty>(string? ColumnName, Func<string, TProperty>? ColumnValueConversion)
{
    public static PropertyConfiguration<T, TProperty> Default => new(null, default);

    public PropertyConfiguration<T, TProperty> MappedToColumn(string columnName) =>
        this with { ColumnName = columnName };

    public PropertyConfiguration<T, TProperty> WithColumnConversion(Func<string, TProperty> conversion) =>
        this with { ColumnValueConversion = conversion };
}