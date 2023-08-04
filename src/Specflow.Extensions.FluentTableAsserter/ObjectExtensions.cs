using System;
using System.Collections.Generic;
using System.Linq;
using Specflow.Extensions.FluentTableAsserter.Asserters;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter;

public static class ObjectExtensions
{
    public static IFluentAsserterInitialization<TElement> InstanceShouldBeEquivalentToTable<TElement>(
        this TElement actualElement,
        Table table
    )
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

        return new FluentInstanceAsserter<TElement>(table, actualElement);
    }
}