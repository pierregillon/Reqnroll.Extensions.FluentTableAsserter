using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Specflow.Extensions.FluentTableAsserter;

public static class StringExtensions
{
    public static bool EqualsHumanized(this string input1, string input2) =>
        FromHumanizedToNormalized(input1) == FromHumanizedToNormalized(input2);

    private static string FromHumanizedToNormalized(string humanizedString) =>
        string.IsNullOrWhiteSpace(humanizedString)
            ? string.Empty
            : Regex.Replace(humanizedString.RemoveDiacritics()
                .ToLowerInvariant(), "[^a-z0-9,]+", "");

    private static string RemoveDiacritics(this string text)
    {
        var str = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var ch in str.Where(ch => CharUnicodeInfo.GetUnicodeCategory((char)ch) != UnicodeCategory.NonSpacingMark))
        {
            stringBuilder.Append(ch);
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
}