using Specflow.Extensions.FluentTableAsserter;
using TechTalk.SpecFlow;

namespace Examples;

[Binding]
public class CustomerSteps
{
    private readonly List<Customer> _customers = new();

    [When(@"I register the customer ""(.*)"" with email address ""(.*)""")]
    public void WhenIRegisterTheCustomerWithEmailAddress(string name, string emailAddress) =>
        _customers.Add(new Customer(name, emailAddress));

    [Then(@"the customer list is")]
    public void ThenTheCustomerListIs(Table table)
        => table
            .ShouldMatch(_customers)
            .WithProperty(x => x.Name)
            .WithProperty(x => x.Name, o => o
                .WithColumnName("Full name")
            )
            .WithProperty(x => x.EmailAddress)
            .AssertEquivalent();
}

internal record Customer(string Name, string EmailAddress);