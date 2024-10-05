using System;

namespace Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

public class ObjectToAssertCannotBeACollectionException() : Exception(GenericMessage)
{
    private const string GenericMessage =
        $"You cannot call '{nameof(ObjectExtensions.ObjectShouldBeEquivalentToTable)}' with a collection. "
        + $"Make sure it is a simple object or use '{nameof(EnumerableExtensions.CollectionShouldBeEquivalentToTable)}' to assert your collection of items.";
}