Feature: Customer registration

Scenario: List all registered customers
    When I register the customer "John Doe" with email address "john.doe@gmail.com"
    Then the customer list is
      | Name     | Email address      |
      | John Doe | john.doe@gmail.com |
    And the customer list is
      | Name     |
      | John Doe |
    And the customer list is
      | Full name |
      | John Doe  |
    And the customer list is
      | Email address      |
      | john.doe@gmail.com |