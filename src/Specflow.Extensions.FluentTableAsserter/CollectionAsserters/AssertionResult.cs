using System.Collections;
using System.Linq;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public record AssertionResult(bool IsSuccess, string MemberName, object? ActualValue, object? ExpectedValue)
{
    public string StringActualValue => ActualValue is not string && ActualValue is IEnumerable enumerable
        ? $"[{PrintItems(enumerable)}]"
        : $"'{ActualValue}'";

    public string StringExpectedValue => ExpectedValue is not string && ExpectedValue is IEnumerable enumerable
        ? $"[{PrintItems(enumerable)}]"
        : $"'{ExpectedValue}'";

    public static AssertionResult Success
        => new(true, string.Empty, null, null);

    public static AssertionResult Fail(string memberName, object? actualValue, object? expectedValue)
        => new(false, memberName, actualValue, expectedValue);

    private static string PrintItems(IEnumerable enumerable)
    {
        const string emptyChar = "''";
        const string separator = ";";

        var normalizedArray = enumerable
            .Enumerate()
            .Select(x => x?.ToString())
            .Select(x => x == string.Empty ? emptyChar : x)
            .ToArray();

        return string.Join($" {separator} ", normalizedArray);
    }
}