using System.Collections;
using System.Linq;

namespace Specflow.Extensions.FluentTableAsserter.Asserters;

public record AssertionResult(bool IsSuccess, string MemberName, object? ActualValue)
{
    public string? StringActualValue => ActualValue is not string && ActualValue is IEnumerable enumerable
        ? string.Join(", ", enumerable.Enumerate().Select(x => x?.ToString()))
        : ActualValue?.ToString();

    public static AssertionResult Success
        => new(true, string.Empty, null);

    public static AssertionResult Fail(string memberName, object? actualValue)
        => new(false, memberName, actualValue);
}