using System;

namespace Specflow.Extensions.FluentTableAsserter;

public class TableRowCountIsDifferentThanElementCountException : Exception
{
    public TableRowCountIsDifferentThanElementCountException(int rowCount, int elementCount)
        : base($"Table row count ({rowCount}) is different than element count ({elementCount})")
    {
    }
}