using System;
using System.Collections.Generic;
using Specflow.Extensions.FluentTableAsserter.Asserters;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter;

public static class TableExtensions
{
    [Obsolete("Use '.ShouldBeEquivalentToTable(table)' instead, from the element collection to assert.")]
    public static IFluentAsserterInitialization<TElement> ShouldMatch<TElement>(
        this Table table,
        IEnumerable<TElement> actualElements
    )
    {
        if (actualElements == null)
        {
            throw new ArgumentNullException(nameof(actualElements));
        }

        return new FluentCollectionAsserter<TElement>(table, actualElements);
    }
}