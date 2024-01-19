using System;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;

public class TableRowCountIsDifferentThanElementCountException(int rowCount, Type type, int elementCount)
    : Exception($"Table row count ({rowCount}) is different than '{type.Name}' count ({elementCount})");