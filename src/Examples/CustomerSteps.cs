using FluentAssertions;
using Specflow.Extensions.FluentTableAsserter;
using TechTalk.SpecFlow;

namespace Examples;

[Binding]
public class CustomerSteps
{
    private readonly ScenarioInfo _scenarioInfo;
    private readonly List<Customer> _customers = new();
    private Exception? _error;

    public CustomerSteps(ScenarioInfo scenarioInfo) => _scenarioInfo = scenarioInfo;

    [When(@"I register the customer ""(.*)"" with email address ""(.*)""")]
    public void WhenIRegisterTheCustomerWithEmailAddress(string name, string emailAddress) =>
        _customers.Add(new Customer(name, emailAddress));

    [When(@"asserting the customer list with")]
    [Then(@"the customer list is")]
    public void ThenTheCustomerListIs(Table table)
    {
        if (!_scenarioInfo.Tags.Contains("ErrorHandling"))
        {
            AssertTableValid(table);
            return;
        }

        try
        {
            AssertTableValid(table);
            throw new InvalidOperationException("Specflow: error should have occurred");
        }
        catch (Exception e)
        {
            _error = e;
        }
    }

    [Then(@"an error occurred with ""(.*)""")]
    public void ThenAnErrorOccurredWith(string errorMessage)
    {
        _error.Should().NotBeNull($"An error was expected to occurred with message '{errorMessage}'.");
        _error!.Message.Should().Be(errorMessage);
    }

    private void AssertTableValid(Table table) => table
        .ShouldMatch(_customers)
        .WithProperty(x => x.FullName)
        .WithProperty(x => x.FullName, o => o.MappedToColumn("Name"))
        .WithProperty(x => x.EmailAddress)
        .WithProperty(x => x.EmailAddress, o => o.MappedToColumn("Address"))
        .AssertEquivalent();
}

internal record Customer(string FullName, string EmailAddress);