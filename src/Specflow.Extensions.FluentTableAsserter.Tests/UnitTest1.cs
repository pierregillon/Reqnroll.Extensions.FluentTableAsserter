using FluentAssertions;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.Tests;

public class TableAssertions
{
    public class ComparingTableAndDataWithSameColumnsAsProperties
    {
        private readonly Table _expectedTable;
        private readonly List<Person> _actualPersons;

        public ComparingTableAndDataWithSameColumnsAsProperties()
        {
            _expectedTable = new Table("First name", "Last name");
            _actualPersons = new List<Person>();
        }

        [Fact]
        public void Does_not_throw_when_values_are_equivalent()
        {
            _expectedTable.AddRow("John", "Doe");
            _actualPersons.Add(new Person("John", "Doe"));

            var assertion = () => _expectedTable
                .ShouldMatch(_actualPersons)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.LastName)
                .AssertEquivalent();

            assertion
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Throws_when_one_value_is_different()
        {
            _expectedTable.AddRow("John", "Doe");
            _actualPersons.Add(new Person("Jonathan", "Doe"));

            var assertion = () => _expectedTable
                .ShouldMatch(_actualPersons)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.LastName)
                .AssertEquivalent();

            assertion
                .Should()
                .Throw<ExpectedTableNotEquivalentToDataException>()
                .WithMessage(
                    "At index 0, 'FirstName' actual data is 'Jonathan' but should be 'John' from column 'First name'."
                );
        }
    }


    public record Person(string FirstName, string LastName);
}