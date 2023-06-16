namespace Specflow.Extensions.FluentTableAsserter;

public record PropertyConfiguration(string? ColumnName)
{
    public static PropertyConfiguration Default => new((string?)null);

    public PropertyConfiguration WithColumnName(string columnName) =>
        this with { ColumnName = columnName };
}