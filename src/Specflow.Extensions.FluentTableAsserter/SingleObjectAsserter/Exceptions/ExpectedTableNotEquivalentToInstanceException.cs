using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class ExpectedTableNotEquivalentToInstanceException : Exception
{
    public ExpectedTableNotEquivalentToInstanceException(
        string memberName,
        object? actualValue,
        string columnName,
        string expectedValue
    )
        : base(
            $"'{memberName}' actual data is '{actualValue}' but should be '{expectedValue}' from column '{columnName}'.")
    {
    }
}