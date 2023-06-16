using Specflow.Extensions.FluentTableAsserter.Asserters;

namespace Specflow.Extensions.FluentTableAsserter.Properties;

public interface IPropertyDefinition<in T>
{
    AssertionResult AssertEquivalent(string expectedValue, T element);
    bool IsMappedTo(string columnName);
}