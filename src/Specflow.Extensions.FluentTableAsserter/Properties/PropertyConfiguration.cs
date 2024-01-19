using System;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public record PropertyConfiguration(
    string? ColumnName,
    Func<string, object?>? ColumnToPropertyConversion,
    Func<object?, object?>? PropertyConversion
)
{
    public static PropertyConfiguration Default => new(null, null, null);
}