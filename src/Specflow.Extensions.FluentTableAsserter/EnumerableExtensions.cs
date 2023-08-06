using System;
using System.Collections;
using System.Collections.Generic;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter;

public static class EnumerableExtensions
{
    public static ICollectionFluentAsserterInitialization<TElement> ShouldBeEquivalentToTable<TElement>(
        this IEnumerable<TElement> actualElements,
        Table table
    )
    {
        if (actualElements == null)
        {
            throw new ArgumentNullException(nameof(actualElements));
        }

        return new CollectionFluentAsserter<TElement>(table, actualElements);
    }

    internal static IEnumerable<object?> Enumerate(this IEnumerable enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext()) yield return enumerator.Current;
    }
}