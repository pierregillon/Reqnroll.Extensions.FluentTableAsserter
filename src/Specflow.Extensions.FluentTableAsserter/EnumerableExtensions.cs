using System;
using System.Collections.Generic;
using Specflow.Extensions.FluentTableAsserter.Asserters;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter;

public static class EnumerableExtensions
{
    public static IFluentAsserterInitialization<TElement> ShouldBeEquivalentToTable<TElement>(
        this IEnumerable<TElement> actualElements,
        Table table
    )
    {
        if (actualElements == null)
        {
            throw new ArgumentNullException(nameof(actualElements));
        }

        return new FluentCollectionAsserter<TElement>(table, actualElements);
    }
}