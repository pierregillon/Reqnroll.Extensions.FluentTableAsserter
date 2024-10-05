using Reqnroll;
using Reqnroll.Extensions.FluentTableAsserter;

namespace Examples.Steps;

[Binding]
public class EmailSteps
{
    private readonly ErrorDriver _errorDriver;
    private Email? _receivedEmail;

    public EmailSteps(ErrorDriver errorDriver) => _errorDriver = errorDriver;

    [When(@"I receive an email with")]
    public void WhenIReceiveAnEmailWith(Table table)
    {
        var singleRow = table.Rows.Single();

        _receivedEmail = new Email(
            singleRow["FromEmail"],
            singleRow["ToEmail"],
            singleRow["Subject"],
            singleRow["PlainText"],
            int.Parse(singleRow["AttachmentCount"])
        );
    }

    [When(@"asserting the email properties with")]
    [Then(@"the received email is")]
    public void WhenAssertingTheEmailPropertiesWith(Table table) =>
        _errorDriver.TryExecute(() => AssertTableValid(table));

    private void AssertTableValid(Table table) => _receivedEmail!
        .ObjectShouldBeEquivalentToTable(table)
        .WithProperty(x => x.FromEmail)
        .WithProperty(x => x.FromEmail, x => x.ComparedToField("From"))
        .WithProperty(x => x.ToEmail)
        .WithProperty(x => x.ToEmail, x => x.ComparedToField("To"))
        .WithProperty(x => x.Subject)
        .WithProperty(x => x.PlainText)
        .WithProperty(x => x.PlainText, x => x.ComparedToField("Text"))
        .WithProperty(x => x.AttachmentCount)
        .Assert();
}

internal record Email(string FromEmail, string ToEmail, string Subject, string PlainText, int AttachmentCount);