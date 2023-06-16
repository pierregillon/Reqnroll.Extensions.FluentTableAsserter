Feature: TestFeature
In order to compare my data and my expected table
As a developer
I want to have an assertion library that correctly works

Scenario: Table and data with same properties are equivalent
    Given the following table
      | First name | Last name |
      | John       | Doe       |
    And the following persons
      | First name | Last name |
      | John       | Doe       |
    When I assert the table with the data
    Then table and data are equivalent