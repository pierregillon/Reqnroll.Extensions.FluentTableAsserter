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
- avoid creating records for every single table to rehydrate
- tend to be ubiquitous language centric (clever string parsing as 'human readable')
- make column declaration optional in gherkin, in order to focus the columns that matter for the scenario

## Examples

For the following scenario:

```gherkin
Scenario: List all registered customers
    When I register the customer "John Doe" with email address "john.doe@gmail.com"
    And I register the customer "Sam Doe" with email address "sam.doe@gmail.com"
    Then the customer list is
      | Name     | Email address      |
      | John Doe | john.doe@gmail.com |
      | Sam Doe  | sam.doe@gmail.com  |
```

You can write the assertion like this:

```csharp
[Then(@"the customer list is")]
    public void ThenTheCustomerListIs(Table table)
        => table
            .ShouldMatch(_customers)
            .WithProperty(x => x.Name)
            .WithProperty(x => x.EmailAddress)
            .AssertEquivalent();
```

Columns are automatically determine based on property names.
'EmailAddress' works but it is also permissive, ´Email adress' works too
and is recommended because it is closer than natural language.

Later in your scenarios, if you need to assert only customer names, you can simply do:

For the following scenario:

```gherkin
Scenario: Other scenario
    Given ...
    Then the customer list is
      | Name     |
      | John Doe |
      | Sam Doe  |
```

## Remaining tasks

- [ ] natively handle enumeration without converter
- [ ] handle single object assertion (instead of list)
- [ ] add more examples
