# Reqnroll Fluent Table Asserter

![Status](https://github.com/pierregillon/Reqnroll.Extensions.FluentTableAsserter/actions/workflows/dotnet.yml/badge.svg)
![Version](https://img.shields.io/badge/dynamic/xml?color=blue&label=version&prefix=v&query=//Project/PropertyGroup/Version/text()&url=https://raw.githubusercontent.com/pierregillon/Reqnroll.Extensions.FluentTableAsserter/main/src/Reqnroll.Extensions.FluentTableAsserter/Reqnroll.Extensions.FluentTableAsserter.csproj)
![Nuget](https://img.shields.io/badge/Nuget-available%20-green)

A reqnroll extension library to simplify table assertion with fluent code.

## Installation

[Nuget package](https://www.nuget.org/packages/Crafty.Reqnroll.Extensions.FluentTableAsserter):

    dotnet add package Crafty.Reqnroll.Extensions.FluentTableAsserter

## Why?

Asserting Reqnroll table can be very painful in large scale application.
Even
if [Reqnroll.Assist Helpers](https://docs.reqnroll.org/projects/reqnroll/en/latest/Bindings/Reqnroll-Assist-Helpers.html)
is a good start to simplify data rehydration from table, it is not very flexible.

The idea to this library is:

- very little code required
- can be extended with **extra configuration**
- avoid creating `record` or `class` for every single table to rehydrate
- tend to be **ubiquitous language centric** (clever string parsing from *human readable* input)
- make column declaration **optional** in gherkin, in order to declare only the columns that are relevant for a scenario

## Example: collection comparison

You can compare a collection with a table.
Headers represent the property names and rows represent the values of the items.

For example, you can write the gherkin assertion:

```gherkin
Scenario: List all registered customers
    Then the customer list is
      | Name      | Email address       | Job                   |
      | John Doe  | john.doe@gmail.com  | Scientist             |
      | Sam Smith | sam.smith@gmail.com | Chief product officer |
```

With the assertion code:

```csharp
[Then(@"the customer list is")]
public void ThenTheCustomerListIs(Table table) => 
    _customers
        .CollectionShouldBeEquivalentToTable(table)
        .WithProperty(x => x.Name)
        .WithProperty(x => x.EmailAddress)
        .WithProperty(x => x.Job)
        .Assert();
```

When the collection is:

```csharp
var customers = new[]
{
    new Customer("John Doe", "john.doe@gmail.com", Job.Scientist),
    new Customer("Sam Smith", "sam.smith@gmail.com", Job.ChiefProductOfficer)
};
```

```csharp
internal record Customer(
    string Name, 
    string EmailAddress, 
    Job Job
 );

public enum Job
{
    Scientist,
    ChiefProductOfficer
}
```

You can find more example [here](./src/Examples).

## Example: object comparison

You can also compare a **single object** with a Reqnroll Table.
The first column should represent the field names, and the second column the values.

For example, you can write the gherkin assertion:

```gherkin
Scenario: Show email details
    Then the received email is
      | Field           | Value                             |
      | From email      | sender@company.com                |
      | To email        | receiver@company.com              |
      | Subject         | Provide schedule                  |
      | Plain text      | Hi,                               |
      | Plain text      | Can you provide me your schedule? |
      | Plain text      | Thanks.                           |
      | AttachmentCount | 3                                 |
```

With the assertion code:

```csharp
[Then(@"the received email is")]
public void WhenAssertingTheEmailPropertiesWith(Table table) => 
    _receivedEmail
        .ObjectShouldBeEquivalentToTable(table)
        .WithProperty(x => x.FromEmail)
        .WithProperty(x => x.ToEmail)
        .WithProperty(x => x.Subject)
        .WithProperty(x => x.PlainText)
        .WithProperty(x => x.AttachmentCount)
        .Assert();
```

When the object is:

```csharp
var email = new Email(
    "sender@company.com",
    "receiver@company.com",
    "Provide schedule",
    "Hi,\nCan you provide me your schedule?\nThanks.",
    3
);
```

```csharp
internal record Email(
    string FromEmail,
    string ToEmail,
    string Subject,
    string PlainText,
    int AttachmentCount
);
```

You can find more example [here](./src/Examples).

## Mapping between columns and properties

The table asserter is **smart** 🤓 and try to determine column name of the table, based on
the **mapped property names**.

`EmailAddress` field => `EmailAddress` column

Or to more readable set of words:

`EmailAddress` field => `Email address` column

`EmailAddress` field => `email address` column

The same principle is applied for parsing enum values : `ChiefProductOfficer` value works
but also `Chief product officer`.

It allows to have gherkin scenario closer to **natural language**.

## Override default comparison behaviour

In certain cases, you would want to override the column that is compared to the property.
A second argument of `.WithProperty()` allows you to provide a delegate to configure the `PropertyConfiguration` object.

### Override column name

```csharp
.WithProperty(x => x.Name, o => o.ComparedToColumn("FullName"))
```

It is useful when your gherkin language is different than english but your classes and records still are in english.

> 💡 Remember in **Domain Driven Development guidelines**, a strong objective is to share the **same language across the
team / company**, from
> domain experts to developers. The code must be aligned to domain specific terms. So in the example, if we decided a *
*customer**
> has a **full name**
> instead of a **name**, it is preferable to rename the property `Name` into `FullName` instead of overriding the mapped
> column.

### Define conversion delegate

If the cell value (a string) cannot be converted to the property type, you must define how the table asserter will
converted it, by providing a **conversion delegate**.

```csharp
.WithProperty(
    x => x.Price,
    o => o.WithCellToPropertyConversion(columnValue => ...)
)
```

For example, given the following `Price` record.

```csharp
 public record Price(decimal Amount, string Symbol);
```

If the table looks

```gherkin
Scenario: Created products are listed
    When I create the product "Black jacket" for 100 USD
    Then the product list is
      | Name         | Price |
      | Black jacket | $100  |
```

You can define the conversion as:

```csharp
.WithProperty(
    x => x.Price,
    o => o.WithCellToPropertyConversion(Price.Parse)
)
```

### Define column transformation

When you have complex objects with wrapped objects, you may want to provide transformation logic within the **property
declaration**.

It works fine, however, error message can be a little bit tricky to understand.

Example:

```csharp
.WithProperty(
    x => x.Customers.Select(c => c.Name),
    o => o
        .ComparedToColumn("Customers")
        .SplitCellValueBySeparator()
)
```

Error message example:
> "At index 0, 'Customers.Select(c => c.Name)' actual data is [John Doe ; Erika Doe] but should
> be [John Doe2 ; Erika Doe]
> from column 'Customers'."

To avoid this, you can add a property transformation `WithPropertyTransformation`:

```csharp
.WithProperty(
    x => x.Customers,
    o => o
        .WithPropertyTransformation(x => x.Select(c => c.Name))
        .SplitCellValueBySeparator()
)
```

Better error message:
> "At index 0, 'Customers' actual data is [John Doe ; Erika Doe] but should be [John Doe2 ; Erika Doe] from column '
> Customers'."

## Optional columns

All columns are **optional by default**, so you don't need to specify them all in all
your scenarios. **The ones you specify are used to assert your data**. Depending on your
scenario, you can specify only the ones that are relevant.

From the previous example, a new customer deletion scenario can be asserted only with the column
`Name`, we can volontary remove the `EmailAddress`: because it is useless here, the `Name`
is enough.

```gherkin
Scenario: Deleted customers are not listed anymore
    When I delete the customer "John Doe"
    Then the customer list is
      | Name      |
      | Sam Smith |
```

## Roadmap

- [x] natively handle enumeration without converter
- [x] handle single object assertion (instead of list)
- [ ] protect if no column match any property defined
- [x] add more examples
- [ ] default converters : string => date (sql format), ...
- [x] reversed converter from value to column value
- [x] Provide default list comparison delegate converter
- [ ] handle enum flags
- [ ] provide examples on ColumnValueConversion
- [ ] provide examples on chained property expression to assert sub elements
- [ ] automatic conversion using implicit operator converter
- [ ] configure to use regex as assertion method on string
- [x] better assert error when chaining method on property (Obj.MyProperty.ToString())
- [ ] better assert error by providing the table that would match
