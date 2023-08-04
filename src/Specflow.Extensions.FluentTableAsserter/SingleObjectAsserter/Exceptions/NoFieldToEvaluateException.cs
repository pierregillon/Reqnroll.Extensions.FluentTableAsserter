using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class NoFieldToEvaluateException : Exception
{
    private const string GenericMessage = "You must define at least one field to assert the object to.";

    public NoFieldToEvaluateException() : base(GenericMessage)
    {
    }
}