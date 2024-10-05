using System;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class NoFieldToEvaluateException() : Exception(GenericMessage)
{
    private const string GenericMessage = "You must define at least one field to assert the object to.";
}