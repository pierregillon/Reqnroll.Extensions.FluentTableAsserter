using System;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;

public class TableRowCountIsDifferentThanElementCountException<T> : Exception
{
    public TableRowCountIsDifferentThanElementCountException(int rowCount, int elementCount)
        : base($"Table row count ({rowCount}) is different than '{typeof(T).Name}' count ({elementCount})")
    {
    }
}