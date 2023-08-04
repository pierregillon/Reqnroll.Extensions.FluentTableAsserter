using System;

namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public interface IFluentAsserter<T> : IFluentAsserterInitialization<T>
{
    IFluentAsserter<T> IgnoringColumn(string columnName);

    [Obsolete("Use 'Assert()' method")]
    void AssertEquivalent();

    void Assert();
}