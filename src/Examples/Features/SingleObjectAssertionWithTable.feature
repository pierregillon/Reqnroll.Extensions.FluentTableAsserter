Feature: Single object assertion with table
In order to assert the system works as expected
As a developer
I want to be able to compare an object to a gherkin table and get clear error message in case of failed assertion

Background:
    When I receive an email with
      | FromEmail          | ToEmail              | Subject          | PlainText                                       | AttachmentCount |
      | sender@company.com | receiver@company.com | Provide schedule | Hi,\nCan you provide me your schedule?\nThanks. | 3               |

@ErrorHandling
Scenario: Assertion fails when more than 2 columns
    When asserting the email properties with
      | Field | Value | Origin |
    Then an error occurred with "You must define strictly 2 columns: first of the field name (ie: Field), second for field value (ie: Value). Column names are flexible."

@ErrorHandling
Scenario: Assertion fails when single column
    When asserting the email properties with
      | Field |
    Then an error occurred with "You must define strictly 2 columns: first of the field name (ie: Field), second for field value (ie: Value). Column names are flexible."

@ErrorHandling
Scenario: Assertion fails when to properties defined
    When asserting the email properties with
      | Field | Value |
    Then an error occurred with "You must define at least one field to assert the object to."

@ErrorHandling
Scenario: Assertion fails when some properties are unknown
    When asserting the email properties with
      | Field | Value |
      | Delay | 1     |
      | Year  | 2023  |
    Then an error occurred with "The column 'Delay' has not been mapped to any property of class 'Email'."

@ErrorHandling
Scenario: Assertion fails when value is different as expected
    When asserting the email properties with
      | Field     | Value               |
      | FromEmail | sender2@company.com |
    Then an error occurred with "'FromEmail' actual data is 'sender@company.com' but should be 'sender2@company.com' from column 'FromEmail'."

Scenario: Assertion passes using exact property names
    Then the received email is
      | Field           | Value                                           |
      | FromEmail       | sender@company.com                              |
      | ToEmail         | receiver@company.com                            |
      | Subject         | Provide schedule                                |
      | PlainText       | Hi,\nCan you provide me your schedule?\nThanks. |
      | AttachmentCount | 3                                               |

Scenario: Assertion passes using human readable property names
    Then the received email is
      | Field            | Value                                           |
      | From email       | sender@company.com                              |
      | To email         | receiver@company.com                            |
      | Subject          | Provide schedule                                |
      | Plain text       | Hi,\nCan you provide me your schedule?\nThanks. |
      | Attachment count | 3                                               |

Scenario: Assertion passes using alternative property names
    Then the received email is
      | Field           | Value                                           |
      | From            | sender@company.com                              |
      | To              | receiver@company.com                            |
      | Subject         | Provide schedule                                |
      | Text            | Hi,\nCan you provide me your schedule?\nThanks. |
      | AttachmentCount | 3                                               |

Scenario: Assertion passes using only some property names
    Then the received email is
      | Field     | Value                |
      | FromEmail | sender@company.com   |
      | ToEmail   | receiver@company.com |

Scenario: Assertion passes when using multiple same field as multiline expectation
    Then the received email is
      | Field | Value                             |
      | Text  | Hi,                               |
      | Text  | Can you provide me your schedule? |
      | Text  | Thanks.                           |