namespace Specflow.Extensions.FluentTableAsserter;

public record AssertionResult(bool IsSuccess, string MemberName, object? ActualValue)
{
    public static AssertionResult Success
        => new(true, string.Empty, null);

    public static AssertionResult Fail(string memberName, object? actualValue)
        => new(false, memberName, actualValue);
}