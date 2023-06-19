using System.Collections;
using System.Collections.Generic;

namespace Specflow.Extensions.FluentTableAsserter.Asserters;

public static class EnumerableExtensions
{
    public static IEnumerable<object?> Enumerate(this IEnumerable enumerable)
    {
        var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext()) yield return enumerator.Current;
    }
}