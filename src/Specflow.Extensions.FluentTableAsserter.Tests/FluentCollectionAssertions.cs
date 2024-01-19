using FluentAssertions;
using Specflow.Extensions.FluentTableAsserter.CollectionAsserters.Exceptions;
using Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;
using Specflow.Extensions.FluentTableAsserter.SingleObjectAsserter;
using TechTalk.SpecFlow;

namespace Specflow.Extensions.FluentTableAsserter.Tests;

public abstract class FluentCollectionAssertions
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
        new List<Person>()
            .CollectionShouldBeEquivalentToTable(new Table(""some header""))
            .Assert();
    }

    public record Person;
}

";

            code
                .Should()
                .NotCompile()
                .WithErrors(
                    "'ICollectionFluentAsserterInitialization<UserCode.Person>' does not contain a definition for 'Assert' "
                    + "and no accessible extension method 'Assert' accepting a first argument of type "
                    + "'ICollectionFluentAsserterInitialization<UserCode.Person>' could be found (are you missing a "
                    + "using directive or an assembly reference?)"
                );
        }

        [Fact]
        public void Fails_with_null_list()
        {
            List<Person> persons = null!;

            var wrongAction = () => persons.CollectionShouldBeEquivalentToTable(SomeTable);

            wrongAction
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("Value cannot be null. (Parameter 'actualElements')");
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private record Person;
    }

    public class ColumnsCompatibilityCheck
    {
        private static readonly IEnumerable<Person> EmptyPersonList = Array.Empty<Person>();

        [Fact]
        public void Fails_when_table_has_columns_that_has_not_been_mapped_to_element_property()
        {
            var table = new Table("Test");

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
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
            var table = new Table("FirstName", "Test");

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .IgnoringColumn("Test")
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Cannot_declare_multiple_times_same_property()
        {
            var table = new Table("FirstName");

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
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
        public void Cannot_declare_multiple_times_same_property_without_different_column_name(string headerVariation)
        {
            var table = new Table("FirstName");

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName, options => options.ComparedToColumn(headerVariation))
                .Assert();

            action
                .Should()
                .Throw<Exception>();
        }

        [Fact]
        public void Can_declare_multiple_times_same_property_but_with_different_column_name()
        {
            var table = new Table("FirstName", "FirstName2");

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.FirstName, options => options.ComparedToColumn("FirstName2"))
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
            var table = new Table(header);

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
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
            var table = new Table(header);

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName, options => options
                    .ComparedToColumn("MyFirstName"))
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Theory]
        [InlineData("FirstName", "FirstName")]
        [InlineData("FirstName", "First name")]
        [InlineData("First name", "FirstName")]
        public void Fails_when_multiple_columns_with_same_name(string firstColumn, string secondColumn)
        {
            var table = new Table(firstColumn, secondColumn);

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName)
                .Assert();

            action
                .Should()
                .Throw<DuplicateColumnsOrFieldsException>()
                .WithMessage(
                    $"Columns or fields {firstColumn}, {secondColumn} are duplicates: they match the same property definition 'FirstName' of class 'Person'."
                );
        }

        [Fact]
        public void Accepts_multiple_property_mapping_to_the_same_column()
        {
            var table = new Table("Name");

            var action = () => EmptyPersonList
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(x => x.FirstName, options => options.ComparedToColumn("Name"))
                .WithProperty(x => x.LastName, options => options.ComparedToColumn("Name"))
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
        private readonly List<Person> _actualPersons = new();
        private readonly Action _assertion;

        public ComparingRowsAndValues()
        {
            _expectedTable = new Table("FirstName", "LastName");
            _assertion = () => _actualPersons
                .CollectionShouldBeEquivalentToTable(_expectedTable)
                .WithProperty(x => x.FirstName)
                .WithProperty(x => x.LastName)
                .Assert();
        }

        [Fact]
        public void Does_not_throw_when_values_are_equivalent_to_rows()
        {
            _expectedTable.AddRow("John", "Doe");
            _actualPersons.Add(new Person("John", "Doe"));
            _assertion
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Accepts_property_declaration_in_different_order_than_columns()
        {
            _expectedTable.AddRow("John", "Doe");
            _actualPersons.Add(new Person("John", "Doe"));

            var action = () => _actualPersons
                .CollectionShouldBeEquivalentToTable(_expectedTable)
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
            _expectedTable.AddRow("John", "Doe");
            _actualPersons.Add(new Person(actualFirstName, "Doe"));

            _assertion
                .Should()
                .Throw<ExpectedTableNotEquivalentToCollectionItemException>()
                .WithMessage(
                    $"At index 0, 'FirstName' actual data is '{actualFirstName}' but should be 'John' from column 'FirstName'."
                );
        }

        [Fact]
        public void Accepts_empty_string_instead_of_null()
        {
            _expectedTable.AddRow(string.Empty, "Doe");
            _actualPersons.Add(new Person(null!, "Doe"));

            _assertion
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Throws_when_row_count_greater_than_element_count()
        {
            _expectedTable.AddRow("Jonathan", "Doe");
            _expectedTable.AddRow("Test", "test");
            _actualPersons.Add(new Person("Jonathan", "Doe"));

            _assertion
                .Should()
                .Throw<TableRowCountIsDifferentThanElementCountException>()
                .WithMessage("Table row count (2) is different than 'Person' count (1)");
        }

        [Fact]
        public void Throws_when_row_count_smaller_than_element_count()
        {
            _expectedTable.AddRow("Jonathan", "Doe");
            _actualPersons.Add(new Person("Jonathan", "Doe"));
            _actualPersons.Add(new Person("Test", "Test"));

            _assertion
                .Should()
                .Throw<TableRowCountIsDifferentThanElementCountException>()
                .WithMessage("Table row count (1) is different than 'Person' count (2)");
        }

        [Fact]
        public void Accepts_multiple_property_mapping_to_the_same_column()
        {
            _expectedTable.AddRow("Jonathan", "Doe");
            _actualPersons.Add(new Person("Jonathan", "Jonathan"));

            var action = () => _actualPersons
                .CollectionShouldBeEquivalentToTable(_expectedTable)
                .WithProperty(x => x.FirstName, options => options
                    .ComparedToColumn("FirstName"))
                .WithProperty(x => x.LastName, options => options
                    .ComparedToColumn("FirstName"))
                .IgnoringColumn("LastName")
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        private record Person(string FirstName, string LastName);
    }

    public class ColumnValueConvertionToPropertyValue
    {
        private readonly Table _expectedTemperatureTable = new("Value", "Type");
        private readonly List<Temperature> _actualTemperatures = new();

        [Fact]
        public void Throws_when_column_value_cannot_be_converted_to_property_type()
        {
            _expectedTemperatureTable.AddRow("Test", "Celsius");
            _actualTemperatures.Add(new Temperature(100, TemperatureType.Celsius));

            var action = () => _actualTemperatures
                .CollectionShouldBeEquivalentToTable(_expectedTemperatureTable)
                .WithProperty(x => x.Value)
                .IgnoringColumn("Type")
                .Assert();

            action
                .Should()
                .Throw<CannotConvertCellValueToPropertyTypeException>()
                .WithMessageStartingWith(
                    "The value 'Test' cannot be converted to type 'Int32' of property 'Temperature.Value'."
                );
        }

        [Fact]
        public void Accepts_when_column_value_cannot_be_converted_to_property_type_but_a_converter_is_defined()
        {
            _expectedTemperatureTable.AddRow("hundred", "Celsius");
            _actualTemperatures.Add(new Temperature(100, TemperatureType.Celsius));

            var action = () => _actualTemperatures
                .CollectionShouldBeEquivalentToTable(_expectedTemperatureTable)
                .WithProperty(x => x.Value, options => options
                    .WithColumnToPropertyConversion(columnValue => columnValue == "hundred" ? 100 : -1))
                .IgnoringColumn("Type")
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Automatically_parse_value_to_enum()
        {
            _expectedTemperatureTable.AddRow("100", "kelvin");
            _actualTemperatures.Add(new Temperature(100, TemperatureType.Kelvin));

            var action = () => _actualTemperatures
                .CollectionShouldBeEquivalentToTable(_expectedTemperatureTable)
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
            _expectedTemperatureTable.AddRow("100", humanReadable);
            _actualTemperatures.Add(new Temperature(100, TemperatureType.SomeOtherValue));

            var action = () => _actualTemperatures
                .CollectionShouldBeEquivalentToTable(_expectedTemperatureTable)
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
            _expectedTemperatureTable.AddRow("100", "test");
            _actualTemperatures.Add(new Temperature(100, TemperatureType.Kelvin));

            var action = () => _actualTemperatures
                .CollectionShouldBeEquivalentToTable(_expectedTemperatureTable)
                .WithProperty(x => x.Value)
                .WithProperty(x => x.Type)
                .Assert();

            action
                .Should()
                .Throw<CannotParseEnumToEnumValuException>()
                .WithMessage("'test' cannot be parsed to any enum value of type Temperature.");
        }

        private record Temperature(int Value, TemperatureType Type);

        private enum TemperatureType
        {
            Celsius,
            Kelvin,
            SomeOtherValue
        }
    }

    public class PropertyValueTransformation
    {
        [Fact]
        public void Throws_when_field_value_cannot_be_converted_to_property_type()
        {
            var table = BuildTable(
                new[] { "Customer" },
                new[] { "John Doe" }
            );

            var roots = new[]
            {
                new Root(new Customer("John Doe"))
            };

            var action = () => roots
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(x => x.Customer)
                .Assert();

            action
                .Should()
                .Throw<CannotConvertCellValueToPropertyTypeException>()
                .WithMessageStartingWith(
                    "The value 'John Doe' cannot be converted to type 'Customer' of property 'Root.Customer'."
                );
        }

        [Fact]
        public void Accepts_when_field_value_cannot_be_converted_to_property_type_but_a_tranformation_is_defined()
        {
            var table = BuildTable(
                new[] { "Customer" },
                new[] { "John Doe" }
            );

            var customers = new[]
            {
                new Root(new Customer("John Doe"))
            };

            var action = () => customers
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(
                    x => x.Customer,
                    x => x.WithPropertyTransformation(cellValue => cellValue.Name)
                )
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Use_the_new_property_type_from_transformation_to_evaluate_equivalency()
        {
            var table = BuildTable(
                new[] { "Name" },
                new[] { "8" }
            );

            var roots = new[]
            {
                new Root(new Customer("John Doe"))
            };

            var action = () => roots
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(
                    x => x.Customer.Name,
                    x => x.WithPropertyTransformation(property => property.Length)
                )
                .Assert();

            action
                .Should()
                .NotThrow();
        }

        private record Root(Customer Customer);

        private record Customer(string Name);
    }

    public class ArrayConvertion
    {
        [Fact]
        public void Enumerable_property_type_is_comparable_with_table_column_value()
        {
            var table = new Table("Names");
            table.AddRow("john, sam, eric");

            var elements = new List<Details>
            {
                new(new[] { "john", "sam", "eric" })
            };

            elements
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(
                    x => x.Names,
                    o => o.SplitCellValueBySeparator()
                )
                .Assert();
        }

        [Fact]
        public void Order_is_preserved_comparing_enumerable()
        {
            var table = new Table("Names");
            table.AddRow("john, sam, eric");

            var elements = new List<Details>
            {
                new(new[] { "sam", "john", "eric" })
            };

            var action = () => elements
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(
                    x => x.Names,
                    o => o.SplitCellValueBySeparator()
                )
                .Assert();

            action
                .Should()
                .Throw<ExpectedTableNotEquivalentToCollectionItemException>()
                .WithMessage(
                    "At index 0, 'Names' actual data is [sam ; john ; eric] but should be [john ; sam ; eric] from column 'Names'."
                );
        }

        [Fact]
        public void Different_length_fails()
        {
            var table = new Table("Names");
            table.AddRow("john, sam");

            var elements = new List<Details>
            {
                new(new[] { "john", "sam", "eric" })
            };

            var action = () => elements
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(
                    x => x.Names,
                    o => o.SplitCellValueBySeparator()
                )
                .Assert();

            action
                .Should()
                .Throw<ExpectedTableNotEquivalentToCollectionItemException>()
                .WithMessage(
                    "At index 0, 'Names' actual data is [john ; sam ; eric] but should be [john ; sam] from column 'Names'."
                );
        }

        [Fact]
        public void Array_with_empty_value_fails_with_correct_message()
        {
            var table = new Table("Names");
            table.AddRow("");

            var elements = new List<Details>
            {
                new(Array.Empty<string>())
            };

            var action = () => elements
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(
                    x => x.Names,
                    o => o.SplitCellValueBySeparator()
                )
                .Assert();

            action
                .Should()
                .Throw<ExpectedTableNotEquivalentToCollectionItemException>()
                .WithMessage(
                    "At index 0, 'Names' actual data is [] but should be [''] from column 'Names'."
                );
        }

        [Fact]
        public void Rich_object_collection_is_assertable_with_double_conversion()
        {
            var table = BuildTable(
                new[] { "Customers" },
                new[] { "john, sam, eric" }
            );

            var customers = new[]
            {
                new CustomerList(new[]
                {
                    new Customer("john"),
                    new Customer("sam2"),
                    new Customer("eric")
                })
            };

            var action = () => customers
                .CollectionShouldBeEquivalentToTable(table)
                .WithProperty(
                    x => x.Customers,
                    o => o
                        .WithPropertyTransformation(x => x.Select(i => i.Name))
                        .SplitCellValueBySeparator()
                )
                .Assert();

            action
                .Should()
                .Throw<ExpectedTableNotEquivalentToCollectionItemException>()
                .WithMessage(
                    "At index 0, 'Customers' actual data is [john ; sam2 ; eric] but should be [john ; sam ; eric] from column 'Customers'."
                );
        }

        private record Details(IEnumerable<string> Names);

        private record CustomerList(IEnumerable<Customer> Customers);

        private record Customer(string Name);
    }

    private static Table BuildTable(string[] headers, params string[][] rows)
    {
        var table = new Table(headers);

        foreach (var row in rows)
        {
            table.AddRow(row);
        }

        return table;
    }
}