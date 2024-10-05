using System;
using Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter;
using Reqnroll.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;

namespace Reqnroll.Extensions.FluentTableAsserter;

public static class ObjectExtensions
{
    public static ISingleObjectFluentAsserterInitialization<TElement> ObjectShouldBeEquivalentToTable<TElement>(
        this TElement actualElement,
        Table table
    ) where TElement : notnull
    {
        if (actualElement == null)
        {
            throw new ArgumentNullException(nameof(actualElement), "Provided object cannot be null.");
        }

        if (actualElement.GetType().IsEnumerableType())
        {
            throw new ObjectToAssertCannotBeACollectionException();
        }

        return new SingleObjectAsserter<TElement>(table, actualElement);
    }
}