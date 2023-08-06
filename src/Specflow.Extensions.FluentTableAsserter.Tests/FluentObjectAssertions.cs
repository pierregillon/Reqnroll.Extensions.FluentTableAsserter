using System.Collections.Immutable;
using FluentAssertions;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;
using Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter.Exceptions;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.Tests;

public abstract class FluentObjectAssertions
{
    public class InstanciatingAssertion
    {
        private static readonly Table SomeTable = new("test");

        public static IEnumerable<object?[]> Collections()
        {
            yield return new object[] { new List<int>() };
            yield return new object[] { Array.Empty<int>() };
            yield return new object[] { new HashSet<int>() };
            yield return new object[] { new ImmutableArray<int>() };
        }

        [Theory]
        [MemberData(nameof(Collections))]
        public void Fails_when_source_is_a_collection(IEnumerable<int> collection)
        {
            Action action = () => collection.ObjectShouldBeEquivalentToTable(SomeTable);

            action
                .Should()
                .Throw<ObjectToAssertCannotBeACollectionException>()
                .WithMessage(
                    $"You cannot call '{nameof(ObjectExtensions.ObjectShouldBeEquivalentToTable)}' with a collection. "
                    + $"Make sure it is a simple object or use '{nameof(EnumerableExtensions.CollectionShouldBeEquivalentToTable)}' to assert your collection of items."
                );
        }

        [Fact]
        public void Fails_to_compile_when_no_property_provided()
        {
            const string code = @"

using TechTalk.SpecFlow;
using Specflow.Extensions.FluentTableAsserter;

namespace Test;

public class UserCode
{
    public static void Execute()
    {
        new Person()
            .ObjectShouldBeEquivalentToTable(new Table(""some header""))
            .Assert();
    }

    public record Person;
}

";

            code
                .Should()
                .NotCompile()
                .WithErrors(
                    "'ISingleObjectFluentAsserterInitialization<UserCode.Person>' does not contain a definition for 'Assert' "
                    + "and no accessible extension method 'Assert' accepting a first argument of type "
                    + "'ISingleObjectFluentAsserterInitialization<UserCode.Person>' could be found (are you missing a "
                    + "using directive or an assembly reference?)"
                );
        }

        [Fact]
        public void Fails_with_null_object()
        {
            Person person = null!;

            var wrongAction = () => person.ObjectShouldBeEquivalentToTable(SomeTable);

            wrongAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Provided object cannot be null. (Parameter 'actualElement')");
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private record Person;
    }

    public class SpecflowFieldCompatibilityCheck
    {
        private static readonly Person SomePerson = new("some name", "some name");

        [Fact]
        public void Fails_when_table_has_fields_that_has_not_been_mapped_to_element_property()
        {
            var table = BuildTable(new FieldValue("Test", SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .Assert();

            action
                .Should()
                .Throw<MissingPropertyDefinitionException>()
                .WithMessage("Column or field 'Test' has not been mapped to any property of class 'Person'.");
        }

        [Fact]
        public void Accepts_when_ignoring_not_wanted_columns()
        {
            var table = BuildTable(new FieldValue("Test", SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .IgnoringField("Test")
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Cannot_declare_multiple_times_same_property()
        {
            var table = BuildTable(new FieldValue("FirstName", SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName)
                .Assert();

            action
                .Should()
                .Throw<PropertyDefinitionAlreadyExistsException>()
                .WithMessage("The same property definition exists: Person.FirstName -> [FirstName]");
        }


        [Theory]
        [InlineData("FirstName")]
        [InlineData("First name")]
        public void Cannot_declare_multiple_times_same_property_without_different_column_name(string fieldName)
        {
            var table = BuildTable(new FieldValue("FirstName", SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName, options => options.ComparedToField(fieldName))
                .Assert();

            action
                .Should()
                .Throw<Exception>();
        }

        [Fact]
        public void Can_declare_multiple_times_same_property_but_with_different_column_name()
        {
            var table = BuildTable(new FieldValue("FirstName", SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName, options => options.ComparedToField("FirstName2"))
                .Assert();

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
            var table = BuildTable(new FieldValue(header, SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .Assert();

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
            var table = BuildTable(new FieldValue(header, SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName, options => options.ComparedToField("MyFirstName"))
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Theory]
        [InlineData("FirstName", "First name")]
        [InlineData("First name", "FirstName")]
        public void Fails_when_multiple_columns_with_same_name(string firstField, string secondField)
        {
            var table = BuildTable(
                new FieldValue(firstField, SomePerson.FirstName),
                new FieldValue(secondField, SomePerson.FirstName)
            );

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .Assert();

            action
                .Should()
                .Throw<DuplicateColumnsOrFieldsException>()
                .WithMessage(
                    $"Columns or fields {firstField}, {secondField} are duplicates: they match the same property definition 'FirstName' of class 'Person'."
                );
        }

        [Fact]
        public void Accepts_multiple_property_mapping_to_the_same_column()
        {
            var table = BuildTable(new FieldValue("Name", SomePerson.FirstName));

            var action = () => SomePerson
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName, options => options
                    .ComparedToField("Name"))
                .WithProperty(x => x.LastName, options => options
                    .ComparedToField("Name"))
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private record Person(string FirstName, string LastName);
    }

    public class ComparingRowsAndValues
    {
        private readonly Table _expectedTable;
        private Person? _actualPerson;
        private readonly Action _assertion;

        public ComparingRowsAndValues()
        {
            _expectedTable = new Table("Field", "Value");
            _assertion = () => _actualPerson!
                .ObjectShouldBeEquivalentToTable(_expectedTable)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.LastName)
                .Assert();
        }

        [Fact]
        public void Does_not_throw_when_values_are_equivalent_to_rows()
        {
            _expectedTable.AddRow("FirstName", "John");
            _expectedTable.AddRow("LastName", "Doe");
            _actualPerson = new Person("John", "Doe");
            _assertion
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Accepts_property_declaration_in_different_order_than_columns()
        {
            _expectedTable.AddRow("FirstName", "John");
            _expectedTable.AddRow("LastName", "Doe");
            _actualPerson = new Person("John", "Doe");

            var action = () => _actualPerson
                .ObjectShouldBeEquivalentToTable(_expectedTable)
                .WithProperty(x => x.LastName)
                .WithProperty(x => x.FirstName)
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Theory]
        [InlineData("jonathan")]
        [InlineData("JOHN")]
        [InlineData("john")]
        public void Throws_when_one_value_is_different(string actualFirstName)
        {
            _expectedTable.AddRow("FirstName", "John");
            _expectedTable.AddRow("LastName", "Doe");
            _actualPerson = new Person(actualFirstName, "Doe");

            _assertion
                .Should()
                .Throw<ExpectedTableNotEquivalentToObjectException>()
                .WithMessage(
                    $"'FirstName' actual data is '{actualFirstName}' but should be 'John' from column 'FirstName'."
                );
        }

        [Fact]
        public void Accepts_empty_string_instead_of_null()
        {
            _expectedTable.AddRow("FirstName", string.Empty);
            _expectedTable.AddRow("LastName", "Doe");
            _actualPerson = new Person(null!, "Doe");

            _assertion
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Accepts_multiple_property_mapping_to_the_same_column2()
        {
            _expectedTable.AddRow("FirstName", "John");
            _expectedTable.AddRow("LastName", "Doe");
            _actualPerson = new Person("John", "John");

            var action = () => _actualPerson
                .ObjectShouldBeEquivalentToTable(_expectedTable)
                .WithProperty(
                    x => x.FirstName,
                    o => o.ComparedToField("FirstName")
                )
                .WithProperty(
                    x => x.LastName,
                    o => o.ComparedToField("FirstName")
                )
                .IgnoringField("LastName")
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        private record Person(string FirstName, string LastName);
    }

    public class ColumnValueConvertionToPropertyValue
    {
        [Fact]
        public void Throws_when_column_value_cannot_be_converted_to_property_type()
        {
            var table = BuildTable(
                new FieldValue("Value", "Test"),
                new FieldValue("Type", "Celsius")
            );

            var temperature = new Temperature(100, TemperatureType.Celsius);

            var action = () => temperature
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.Value)
                .IgnoringField("Type")
                .Assert();

            action
                .Should()
                .Throw<CannotConvertCellValueToPropertyTypeException>()
                .WithMessage("The value 'Test' cannot be converted to type 'Int32' of property 'Temperature.Value'");
        }

        [Fact]
        public void Accepts_when_column_value_cannot_be_converted_to_property_type_but_a_converter_is_defined()
        {
            var table = BuildTable(
                new FieldValue("Value", "hundred"),
                new FieldValue("Type", "Celsius")
            );

            var temperature = new Temperature(100, TemperatureType.Celsius);

            var action = () => temperature
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.Value, options => options
                    .WithFieldValueConversion(columnValue => columnValue == "hundred" ? 100 : -1))
                .IgnoringField("Type")
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Automatically_parse_value_to_enum()
        {
            var table = BuildTable(
                new FieldValue("Value", "100"),
                new FieldValue("Type", "kelvin")
            );

            var temperature = new Temperature(100, TemperatureType.Kelvin);

            var action = () => temperature
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.Value)
                .WithProperty(x => x.Type)
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Theory]
        [InlineData("some other value")]
        [InlineData("Some Other Value")]
        public void Automatically_parse_human_readable_value_to_enum(string humanReadable)
        {
            var table = BuildTable(
                new FieldValue("Value", "100"),
                new FieldValue("Type", humanReadable)
            );

            var temperature = new Temperature(100, TemperatureType.SomeOtherValue);

            var action = () => temperature
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.Value)
                .WithProperty(x => x.Type)
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Fails_when_enum_value_invalid()
        {
            var table = BuildTable(
                new FieldValue("Value", "100"),
                new FieldValue("Type", "test")
            );

            var temperature = new Temperature(100, TemperatureType.Kelvin);

            var action = () => temperature
                .ObjectShouldBeEquivalentToTable(table)
                .WithProperty(x => x.Value)
                .WithProperty(x => x.Type)
                .Assert();

            action
                .Should()
                .Throw<CannotParseEnumToEnumValuException<TemperatureType>>()
                .WithMessage("'test' cannot be parsed to any enum value of type TemperatureType.");
        }

        private record Temperature(int Value, TemperatureType Type);

        private enum TemperatureType
        {
            Celsius,
            Kelvin,
            SomeOtherValue
        }
    }

    public class ArrayConvertion
    {
        private readonly Table _table = BuildTable(new FieldValue("Names", "john, sam, eric"));

        [Fact]
        public void Enumerable_property_type_is_comparable_with_table_column_value()
        {
            var element = new Details(new[] { "john", "sam", "eric" });

            element
                .ObjectShouldBeEquivalentToTable(_table)
                .WithProperty(x => x.Names, o => o
                    .WithFieldValueConversion(columnValue => columnValue.Split(',', StringSplitOptions.TrimEntries))
                )
                .Assert();
        }

        [Fact]
        public void Order_is_preserved_comparing_enumerable()
        {
            var element = new Details(new[] { "sam", "john", "eric" });

            var action = () => element
                .ObjectShouldBeEquivalentToTable(_table)
                .WithProperty(x => x.Names, o => o
                    .WithFieldValueConversion(columnValue => columnValue.Split(',', StringSplitOptions.TrimEntries))
                )
                .Assert();

            action
                .Should()
                .Throw<ExpectedTableNotEquivalentToObjectException>()
                .WithMessage(
                    "'Names' actual data is 'sam, john, eric' but should be 'john, sam, eric' from column 'Names'."
                );
        }

        [Fact]
        public void Different_length_fails()
        {
            var element = new Details(new[] { "sam", "john" });

            var action = () => element
                .ObjectShouldBeEquivalentToTable(_table)
                .WithProperty(x => x.Names, o => o
                    .WithFieldValueConversion(columnValue => columnValue.Split(',', StringSplitOptions.TrimEntries))
                )
                .Assert();

            action
                .Should()
                .Throw<ExpectedTableNotEquivalentToObjectException>()
                .WithMessage(
                    "'Names' actual data is 'sam, john' but should be 'john, sam, eric' from column 'Names'."
                );
        }

        private record Details(IEnumerable<string> Names);
    }


    private static Table BuildTable(params FieldValue[] fieldValues)
    {
        var table = new Table("Field", "Value");

        foreach (var fieldValue in fieldValues)
        {
            table.AddRow(fieldValue.FieldName, fieldValue.Value);
        }

        return table;
    }

    private record FieldValue(string FieldName, string Value);
}