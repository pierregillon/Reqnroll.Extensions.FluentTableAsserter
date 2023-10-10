using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class ExpectedTableNotEquivalentToObjectException : Exception
{
    public ExpectedTableNotEquivalentToObjectException(
        string memberName,
        string actualValue,
        string columnName,
        string expectedValue
    )
        : base(
            $"'{memberName}' actual data is {actualValue} but should be {expectedValue} from column '{columnName}'.")
    {
    }
}