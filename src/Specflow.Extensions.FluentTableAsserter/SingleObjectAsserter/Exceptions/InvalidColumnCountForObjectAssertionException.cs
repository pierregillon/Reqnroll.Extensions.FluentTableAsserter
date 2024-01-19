using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class InvalidColumnCountForObjectAssertionException() : Exception(GenericMessage)
{
    private const string GenericMessage =
        "You must define strictly 2 columns: first is the field name (ie: Field), second is the field value (ie: Value). Column names are flexible.";
}