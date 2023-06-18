Feature: Element list assertion with table
In order to assert the system works as expected
As a developer
I want to be able to compare element list to a gherkin table and get clear error message in case of failed assertion

Background:
    When I register the customer "John Doe" with email address "john.doe@gmail.com"
    And I register the customer "Sam Smith" with email address "sam.smith@gmail.com"

Scenario: Assertion works using exact property names
    Then the customer list is
      | FullName  | EmailAddress        |
      | John Doe  | john.doe@gmail.com  |
      | Sam Smith | sam.smith@gmail.com |

Scenario: Assertion works using human readable property names
    Then the customer list is
      | Full name | Email address       |
      | John Doe  | john.doe@gmail.com  |
      | Sam Smith | sam.smith@gmail.com |

Scenario: Assertion works using only some property names
    Then the customer list is
      | Full name |
      | John Doe  |
      | Sam Smith |

Scenario: Assertion works using alternative property names
    Then the customer list is
      | Name      | Address             |
      | John Doe  | john.doe@gmail.com  |
      | Sam Smith | sam.smith@gmail.com |

@ErrorHandling
Scenario: Assertion fails using an unknown column that has not been mapped
    When asserting the customer list with
      | Full name | Birth date |
      | John Doe  | 1989-10-02 |
      | Sam Smith | 1999-11-01 |
    Then an error occurred with "The column 'Birth date' has not been mapped to any property of class 'Customer'."

@ErrorHandling
Scenario: Assertion fails when order is different
    When asserting the customer list with
      | Full name |
      | Sam Smith |
      | John Doe  |
    Then an error occurred with "At index 0, 'FullName' actual data is 'John Doe' but should be 'Sam Smith' from column 'Full name'."

@ErrorHandling
Scenario: Assertion fails when row count is different than element count
    When asserting the customer list with
      | Full name |
      | John Doe  |
    Then an error occurred with "Table row count (1) is different than 'Customer' count (2)"