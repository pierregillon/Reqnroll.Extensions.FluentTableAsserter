namespace Specflow.Extensions.FluentTableAsserter;

public interface IFluentAsserter<T> : IFluentAsserterInitialization<T>
{
    IFluentAsserter<T> IgnoringColumn(string columnName);
    void AssertEquivalent();
}