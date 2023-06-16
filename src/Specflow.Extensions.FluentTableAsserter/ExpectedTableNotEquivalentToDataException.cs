using System;

namespace Specflow.Extensions.FluentTableAsserter;

public class ExpectedTableNotEquivalentToDataException : Exception
{
    public ExpectedTableNotEquivalentToDataException(
        int index,
        string memberName,
        object? actualValue,
        string headerName,
        string expectedValue
    )
        : base(
            $"At index {index}, '{memberName}' actual data is '{actualValue}' but should be '{expectedValue}' from column '{headerName}'.")
    {
    }
}