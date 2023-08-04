Feature: Element list assertion with table
In order to assert the system works as expected
As a developer
I want to be able to compare element list to a gherkin table and get clear error message in case of failed assertion

Background:
    When I register a scientist customer "John Doe" with email address "john.doe@gmail.com"
    And I register a chief product officer customer "Sam Smith" with email address "sam.smith@gmail.com"

Scenario: Assertion passes using exact property names
    Then the customer list is
      | FullName  | EmailAddress        | Job                 |
      | John Doe  | john.doe@gmail.com  | Scientist           |
      | Sam Smith | sam.smith@gmail.com | ChiefProductOfficer |

Scenario: Assertion passes using human readable property names and values
    Then the customer list is
      | Full name | Email address       | Job                   |
      | John Doe  | john.doe@gmail.com  | Scientist             |
      | Sam Smith | sam.smith@gmail.com | Chief product officer |

Scenario: Assertion passes using only some property names
    Then the customer list is
      | Full name |
      | John Doe  |
      | Sam Smith |

Scenario: Assertion passes using alternative property names
    Then the customer list is
      | Name      | Address             |
      | John Doe  | john.doe@gmail.com  |
      | Sam Smith | sam.smith@gmail.com |

@ErrorHandling
Scenario: Assertion fails when value is different than expected one
    When asserting the customer list with
      | Full name |
      | John2 Doe |
      | Sam Smith |
    Then an error occurred with "At index 0, 'FullName' actual data is 'John Doe' but should be 'John2 Doe' from column 'Full name'."

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