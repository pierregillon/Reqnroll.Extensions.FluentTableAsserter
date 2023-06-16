using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyConfiguration<T, TProperty>(string? ColumnName, Func<string, TProperty>? ColumnValueConvertion)
{
    public static PropertyConfiguration<T, TProperty> Default => new(null, default);

    public PropertyConfiguration<T, TProperty> WithColumnName(string columnName) =>
        this with { ColumnName = columnName };

    public PropertyConfiguration<T, TProperty> WithColumnConversion(Func<string, TProperty> convertion) =>
        this with { ColumnValueConvertion = convertion };
}