using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter;

public static class TableExtensions
{
    public static IFluentAsserterInitialization<TElement> ShouldMatch<TElement>(
        this Table table,
        IEnumerable<TElement> actualElements
    )
    {
        if (actualElements == null)
        {
            throw new ArgumentNullException(nameof(actualElements));
        }

        return new FluentAsserter<TElement>(table, actualElements);
    }
}