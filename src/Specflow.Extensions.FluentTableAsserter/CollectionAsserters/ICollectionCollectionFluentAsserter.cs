namespace Specflow.Extensions.FluentTableAsserter.CollectionAsserters;

public interface ICollectionCollectionFluentAsserter<T> : ICollectionFluentAsserterInitialization<T>
{
    ICollectionCollectionFluentAsserter<T> IgnoringColumn(string columnName);

    void Assert();
}