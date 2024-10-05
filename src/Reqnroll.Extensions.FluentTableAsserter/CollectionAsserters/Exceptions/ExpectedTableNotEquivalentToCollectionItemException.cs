using System;

namespace Reqnroll.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;

public class ExpectedTableNotEquivalentToCollectionItemException(
    int index,
    string memberName,
    string actualValue,
    string columnName,
    string expectedValue
) : Exception(
    $"At index {index}, '{memberName}' actual data is {actualValue} but should be {expectedValue} from column '{columnName}'.");