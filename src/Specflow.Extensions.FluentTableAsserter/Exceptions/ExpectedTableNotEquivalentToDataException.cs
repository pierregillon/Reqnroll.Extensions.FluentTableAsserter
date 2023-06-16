using System;

namespace Specflow.Extensions.FluentTableAsserter.Exceptions;

public class ExpectedTableNotEquivalentToDataException : Exception
{
    public ExpectedTableNotEquivalentToDataException(
        int index,
        string memberName,
        object? actualValue,
        string columnName,
        string expectedValue
    )
        : base(
            $"At index {index}, '{memberName}' actual data is '{actualValue}' but should be '{expectedValue}' from column '{columnName}'.")
    {
    }
}