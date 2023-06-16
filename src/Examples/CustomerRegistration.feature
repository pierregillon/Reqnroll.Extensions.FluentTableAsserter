Feature: Customer registration

Scenario: List all registered customers
    When I register the customer "John Doe" with email address "john.doe@gmai.com"
    Then the customer list is
      | Name     | Email address     | Full name |
      | John Doe | john.doe@gmai.com | John Doe  |
    And the customer list is
      | Name     |
      | John Doe |
    And the customer list is
      | Email address     |
      | john.doe@gmai.com |