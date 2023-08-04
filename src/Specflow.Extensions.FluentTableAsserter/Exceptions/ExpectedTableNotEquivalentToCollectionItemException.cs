using System;

namespace Specflow.Extensions.FluentTableAsserter.Exceptions;

public class ExpectedTableNotEquivalentToCollectionItemException : Exception
{
    public ExpectedTableNotEquivalentToCollectionItemException(
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