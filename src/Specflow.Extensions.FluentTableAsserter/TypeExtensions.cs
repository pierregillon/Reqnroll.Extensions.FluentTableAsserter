using System;
using System.Collections;

namespace Specflow.Extensions.FluentTableAsserter;

internal static class TypeExtensions
{
    public static bool IsEnumerableType(this Type type) => type.GetInterface(nameof(IEnumerable)) != null;
}