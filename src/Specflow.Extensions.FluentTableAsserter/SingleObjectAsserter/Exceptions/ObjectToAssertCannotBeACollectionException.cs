using System;

namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class ObjectToAssertCannotBeACollectionException : Exception
{
    private const string GenericMessage =
        $"You cannot call '{nameof(ObjectExtensions.InstanceShouldBeEquivalentToTable)}' with a collection. "
        + $"Make sure it is a simple object or use '{nameof(EnumerableExtensions.ShouldBeEquivalentToTable)}' to assert your collection of items.";

    public ObjectToAssertCannotBeACollectionException() : base(GenericMessage)
    {
    }
}