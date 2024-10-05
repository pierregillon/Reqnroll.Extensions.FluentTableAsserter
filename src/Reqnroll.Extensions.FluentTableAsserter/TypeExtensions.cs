using System;
using System.Collections;

namespace Reqnroll.Extensions.FluentTableAsserter;

internal static class TypeExtensions
{
    public static bool IsEnumerableType(this Type type) => type.GetInterface(nameof(IEnumerable)) != null;
}