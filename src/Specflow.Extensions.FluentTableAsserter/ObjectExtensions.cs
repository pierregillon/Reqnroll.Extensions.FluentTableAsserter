using System;
using System.Collections.Generic;
using System.Linq;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;
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
            throw new ArgumentNullException(nameof(actualElement));
        }

        if (actualElement.GetType().GetInterfaces()
            .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            throw new InstanceToAssertCannotBeACollectionException();
        }

        return new SingleObjectAsserter<TElement>(table, actualElement);
    }
}