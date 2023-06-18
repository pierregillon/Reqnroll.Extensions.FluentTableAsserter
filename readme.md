# Specflow Fluent Table Asserter

![Status](https://github.com/pierregillon/Specflow.Extensions.FluentTableAsserter/actions/workflows/dotnet.yml/badge.svg)
![Version](https://img.shields.io/badge/dynamic/xml?color=blue&label=version&prefix=v&query=//Project/PropertyGroup/Version/text()&url=https://raw.githubusercontent.com/pierregillon/Specflow.Extensions.FluentTableAsserter/main/src/Specflow.Extensions.FluentTableAsserter/Specflow.Extensions.FluentTableAsserter.csproj)
![Nuget](https://img.shields.io/badge/Nuget-available%20-green)

A specflow extension library to simplify table assertion with fluent code.

## Installation

[Nuget package](https://www.nuget.org/packages/Crafty.Specflow.Extensions.FluentTableAsserter):

    dotnet add package Crafty.Specflow.Extensions.FluentTableAsserter

## Why?

Asserting Specflow table can be very painful in large scale application.
Even 
if [SpecFlow.Assist Helpers](https://docs.specflow.org/projects/specflow/en/latest/Bindings/SpecFlow-Assist-Helpers.html)
is a good start to simplify data rehydration from table, it is not very flexible.

The idea to this library is:

- just little code required
- avoid creating `record` for every single table to rehydrate
- tend to be **ubiquitous language centric** (clever string parsing as 'human readable')
- make column declaration optional in gherkin, in order to focus to columns that matter for the scenario

## Examples

For the following scenario:

```gherkin
Scenario: List all registered customers
    When I register a scientist customer "John Doe" with email address "john.doe@gmail.com"
    And I register a chief product officer customer "Sam Smith" with email address "sam.smith@gmail.com"
    Then the customer list is
      | Name      | Email address       | Job                   |
      | John Doe  | john.doe@gmail.com  | Scientist             |
      | Sam Smith | sam.smith@gmail.com | Chief product officer |
```

You can write the following assertion:

```csharp
[Then(@"the customer list is")]
    public void ThenTheCustomerListIs(Table table)
        => table
            .ShouldMatch(_customers)
            .WithProperty(x => x.Name)
            .WithProperty(x => x.EmailAddress)
            .WithProperty(x => x.Job)
            .AssertEquivalent();
```

Where, for the example, `Customer` is:
```csharp
internal record Customer(
    string FullName, 
    string EmailAddress, 
    Job Job
 );

public enum Job
{
    Scientist,
    ChiefProductOfficer
}
```
## Mapping between columns and properties

The table asserter is smart and try to determine column name of the table, based on 
the mapped property names.

`EmailAddress` property is automatically mapped to `EmailAddress` column or to any another more
natural set of words, like `Email address` or `email address` or any other case.

The same is applied for enum values : `ChiefProductOfficer` value works but also `Chief product officer`.

It allows to have gherkin scenario closer to **natural language**.

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

## Remaining tasks

- [x] natively handle enumeration without converter
- [ ] handle single object assertion (instead of list)
- [ ] add more examples
- [ ] reversed converter from value to column value
- [ ] handle enum flags
