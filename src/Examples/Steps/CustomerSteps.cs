using Specflow.Extensions.FluentTableAsserter;
using TechTalk.SpecFlow;

namespace Examples.Steps;

[Binding]
public class CustomerSteps
{
    private readonly ErrorDriver _errorDriver;
    private readonly List<Customer> _customers = new();

    public CustomerSteps(ErrorDriver errorDriver) => _errorDriver = errorDriver;

    [When(@"I register a (.*) customer ""(.*)"" with email address ""(.*)""")]
    public void WhenIRegisterTheCustomerWithEmailAddress(Job job, string name, string emailAddress) =>
        _customers.Add(new Customer(name, emailAddress, job));

    [When(@"asserting the customer list with")]
    [Then(@"the customer list is")]
    public void ThenTheCustomerListIs(Table table) =>
        _errorDriver.TryExecute(() => AssertTableValid(table));

    private void AssertTableValid(Table table) => _customers
        .ShouldBeEquivalentToTable(table)
        .WithProperty(x => x.FullName)
        .WithProperty(x => x.FullName, o => o.ComparedToColumn("Name"))
        .WithProperty(x => x.EmailAddress)
        .WithProperty(x => x.EmailAddress, o => o.ComparedToColumn("Address"))
        .WithProperty(x => x.Job)
        .Assert();
}

internal record Customer(string FullName, string EmailAddress, Job Job);

public enum Job
{
    Scientist,
    ChiefProductOfficer
}