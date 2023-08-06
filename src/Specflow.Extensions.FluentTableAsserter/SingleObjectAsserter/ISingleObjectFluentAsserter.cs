namespace Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;

public interface ISingleObjectFluentAsserter<T> : ISingleObjectFluentAsserterInitialization<T>
{
    ISingleObjectFluentAsserter<T> IgnoringField(string columnName);

    void Assert();
}