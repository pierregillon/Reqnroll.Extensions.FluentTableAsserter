using System;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class ExpectedTableNotEquivalentToObjectException(
    string memberName,
    string actualValue,
    string columnName,
    string expectedValue
)
    : Exception(
        $"'{memberName}' actual data is {actualValue} but should be {expectedValue} from column '{columnName}'.");