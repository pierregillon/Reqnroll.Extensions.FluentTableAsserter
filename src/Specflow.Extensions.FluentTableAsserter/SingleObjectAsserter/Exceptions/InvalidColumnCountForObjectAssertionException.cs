using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class InvalidColumnCountForObjectAssertionException : Exception
{
    private const string GenericMessage =
        "You must define strictly 2 columns: first of the field name (ie: Field), second for field value (ie: Value). Column names are flexible.";

    public InvalidColumnCountForObjectAssertionException() : base(GenericMessage)
    {
    }
}