using System;

namespace Reqnroll.Extensions.FluentTableAsserter.Properties;

public record PropertyConfiguration(
    string? ColumnName,
    Func<string, object?>? ColumnToPropertyConversion,
    Func<object?, object?>? PropertyConversion,
    bool UseStrictEnumerableComparison
)
{
    public static PropertyConfiguration Default => new(null, null, null, true);
}