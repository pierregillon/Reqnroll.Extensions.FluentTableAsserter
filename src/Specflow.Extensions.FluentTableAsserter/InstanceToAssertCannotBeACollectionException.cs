using System;

namespace Specflow.Extensions.FluentTableAsserter;

public class InstanceToAssertCannotBeACollectionException : Exception
{
    private const string GenericMessage =
        $"You cannot call '{nameof(ObjectExtensions.InstanceShouldBeEquivalentToTable)}' with a collection. "
        + $"Make sure it is a simple object or use '{nameof(EnumerableExtensions.ShouldBeEquivalentToTable)}' to assert your collection of items.";

    public InstanceToAssertCannotBeACollectionException() : base(GenericMessage)
    {
    }
}