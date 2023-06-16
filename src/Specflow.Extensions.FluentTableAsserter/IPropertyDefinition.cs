namespace Specflow.Extensions.FluentTableAsserter;

public interface IPropertyDefinition<in T>
{
    AssertionResult AssertEquivalent(string expectedValue, T element);
    bool IsMappedTo(string columnName);
}