using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Specflow.Extensions.FluentTableAsserter.Tests;

[Binding]
public class TableSteps
{
    private Table _table;
    private IEnumerable<Person> _persons;

    [Given(@"the following table")]
    public void GivenTheFollowingTable(Table table) => _table = table;

    [Given(@"the following persons")]
    public void GivenTheFollowingPersons(Table table) => _persons = table.CreateSet<Person>();

    [When(@"I assert the table with the data")]
    public void WhenIAssertTheTableWithTheData() => _table
        .ShouldMatch(_persons)
        .WithProperty(x => x.FirstName)
        .WithProperty(x => x.LastName)
        .AssertEquivalent();
}

public record Person(string FirstName, string LastName);