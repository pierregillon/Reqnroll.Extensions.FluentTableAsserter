using System;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter;

public static class ObjectExtensions
{
    public static IFluentAsserterInitialization<TElement> InstanceShouldBeEquivalentToTable<TElement>(
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