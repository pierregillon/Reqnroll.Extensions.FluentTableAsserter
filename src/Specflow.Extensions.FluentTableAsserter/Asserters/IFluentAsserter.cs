namespace Specflow.Extensions.FluentTableAsserter.Asserters;

public interface IFluentAsserter<T> : IFluentAsserterInitialization<T>
{
    IFluentAsserter<T> IgnoringColumn(string columnName);
    void AssertEquivalent();
}