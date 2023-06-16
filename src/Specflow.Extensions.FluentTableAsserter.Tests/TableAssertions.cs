using FluentAssertions;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.Tests;

public abstract class TableAssertions
{
    public class InstanciatingAssertion
    {
        private static readonly Table SomeTable = new("test");

        [Fact]
        public void Fails_to_compile_when_no_property_provided()
        {
            const string code = @"

using System.Collections.Generic;
using TechTalk.SpecFlow;
using Specflow.Extensions.FluentTableAsserter;

namespace Test;

public class UserCode
{
    public static void Execute()
    {
        new Table(""some header"")
            .ShouldMatch(new List<Person>())
            .AssertEquivalent();
    }

    public record Person;
}

";

            code
                .Should()
                .NotCompile()
                .WithErrors("'Table' does not contain a definition for 'ShouldMatch' and no "
                    + "accessible extension method 'ShouldMatch' accepting a first argument of type 'Table' "
                    + "could be found (are you missing a using directive or an assembly reference?)"
                );
        }

        [Fact]
        public void Fails_with_null_list()
        {
            List<Person> persons = null!;

            var wrongAction = () => SomeTable.ShouldMatch(persons);

            wrongAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Value cannot be null. (Parameter 'actualElements')");
        }

        private record Person;
    }

    public class ColumnsCompatibilityCheck
    {
        [Fact]
        public void Fails_when_table_has_columns_that_has_not_been_mapped_to_element_property()
        {
            var table = new Table("Test");

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName)
                .AssertEquivalent();

            action
                .Should()
                .Throw<MissingColumnDefinitionException>()
                .WithMessage("The column 'Test' has not been mapped to any property of class 'Person'.");
        }

        [Fact]
        public void Accepts_when_ignoring_not_wanted_columns()
        {
            var table = new Table("FirstName", "Test");

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName)
                .IgnoringColumn("Test")
                .AssertEquivalent();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Cannot_declare_multiple_times_same_property()
        {
            var table = new Table("FirstName");

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName)
                .AssertEquivalent();

            action
                .Should()
                .Throw<PropertyDefinitionAlreadyExists>()
                .WithMessage("The same property definition exists: Person.FirstName -> [FirstName]");
        }

        [Theory]
        [InlineData("FirstName")]
        [InlineData("First name")]
        public void Cannot_declare_multiple_times_same_property_without_different_column_name(string headerVariation)
        {
            var table = new Table("FirstName");

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName, options => options.WithColumnName(headerVariation))
                .AssertEquivalent();

            action
                .Should()
                .Throw<Exception>();
        }

        [Fact]
        public void Can_declare_multiple_times_same_property_but_with_different_column_name()
        {
            var table = new Table("FirstName", "FirstName2");

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName, options => options.WithColumnName("FirstName2"))
                .AssertEquivalent();

            action
                .Should()
                .NotThrow();
        }

        [Theory]
        [InlineData("First Name")]
        [InlineData("First name")]
        [InlineData("firstname")]
        [InlineData("first name")]
        [InlineData("FIRST NAME")]
        public void Accepts_column_when_naming_is_equivalent_for_a_human(string header)
        {
            var table = new Table(header);

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName)
                .AssertEquivalent();

            action
                .Should()
                .NotThrow();
        }

        [Theory]
        [InlineData("My First Name")]
        [InlineData("my First name")]
        [InlineData("myfirstname")]
        [InlineData("my first name")]
        [InlineData("MY FIRST NAME")]
        public void Accepts_column_when_a_different_column_name_has_been_configured(string header)
        {
            var table = new Table(header);

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName, options => options
                    .WithColumnName("MyFirstName"))
                .AssertEquivalent();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Accepts_multiple_property_mapping_to_the_same_column()
        {
            var table = new Table("Name");

            var action = () => table
                .ShouldMatch(new List<Person>())
                .WithProperty(x => x.FirstName, options => options
                    .WithColumnName("Name"))
                .WithProperty(x => x.LastName, options => options
                    .WithColumnName("Name"))
                .AssertEquivalent();

            action
                .Should()
                .NotThrow();
        }

        private record Person(string FirstName, string LastName);
    }

    public class ComparingTableAndDataWithSameColumnsAsProperties
    {
        private readonly Table _expectedTable;
        private readonly List<Person> _actualPersons = new();

        public ComparingTableAndDataWithSameColumnsAsProperties() =>
            _expectedTable = new Table("FirstName", "LastName");

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
        public void Accepts_property_declaration_in_different_order_than_columns()
        {
            _expectedTable.AddRow("John", "Doe");
            _actualPersons.Add(new Person("John", "Doe"));

            var action = () => _expectedTable
                .ShouldMatch(_actualPersons)
                .WithProperty(x => x.LastName)
                .WithProperty(x => x.FirstName)
                .AssertEquivalent();

            action
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
                    "At index 0, 'FirstName' actual data is 'Jonathan' but should be 'John' from column 'FirstName'."
                );
        }

        [Fact]
        public void Throws_when_column_value_cannot_be_converted_to_property_type()
        {
            var expectedTemperatureTable = new Table("Value");
            expectedTemperatureTable.AddRow("Test");

            var actualTemperatures = new List<Temperature> { new(100) };

            var action = () => expectedTemperatureTable
                .ShouldMatch(actualTemperatures)
                .WithProperty(x => x.Value)
                .AssertEquivalent();

            action
                .Should()
                .Throw<CannotConvertColumnValueToPropertyTypeException>()
                .WithMessage("The value 'Test' cannot be converted to type 'Int32' of property 'Temperature.Value'");
        }

        [Fact]
        public void Accepts_when_column_value_cannot_be_converted_to_property_type_but_a_converter_is_defined()
        {
            var expectedTemperatureTable = new Table("Value");
            expectedTemperatureTable.AddRow("hundred");

            var actualTemperatures = new List<Temperature> { new(100) };

            var action = () => expectedTemperatureTable
                .ShouldMatch(actualTemperatures)
                .WithProperty(x => x.Value, options => options
                    .WithConversion(columnValue => columnValue == "hundred" ? 100 : -1))
                .AssertEquivalent();

            action
                .Should()
                .NotThrow();
        }

        private record Person(string FirstName, string LastName);

        private record Temperature(int Value);
    }
}