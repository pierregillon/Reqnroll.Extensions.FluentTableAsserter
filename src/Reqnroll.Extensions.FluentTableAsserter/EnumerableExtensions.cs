using System;
using System.Collections;
using System.Collections.Generic;
using Reqnroll.Extensions.FluentTableAsserter.CollectionAsserters;

namespace Reqnroll.Extensions.FluentTableAsserter;

public static class EnumerableExtensions
{
    public static ICollectionFluentAsserterInitialization<TElement> CollectionShouldBeEquivalentToTable<TElement>(
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