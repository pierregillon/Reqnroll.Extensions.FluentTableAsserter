using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Specflow.Extensions.FluentTableAsserter;

internal static class HumanReadableExtensions
{
    public static bool EqualsHumanReadable(this string input1, string input2) =>
        FromHumanReadableToNormalized(input1) == FromHumanReadableToNormalized(input2);

    public static bool TryParseEnum(Type enumType, string humanReadableString, out object result)
    {
        var normalized = FromHumanReadableToNormalized(humanReadableString);
        var isEnumFlag = enumType.GetCustomAttribute(typeof(FlagsAttribute)) != null;

        if (!string.IsNullOrEmpty(normalized) || !isEnumFlag)
        {
            return Enum.TryParse(enumType, normalized, true, out result);
        }

        result = Activator.CreateInstance(enumType)
            ?? throw new InvalidOperationException($"Fail to instanciate {enumType}");

        return true;
    }

    private static string FromHumanReadableToNormalized(string humanizedString) =>
        string.IsNullOrWhiteSpace(humanizedString)
            ? string.Empty
            : Regex.Replace(humanizedString.RemoveDiacritics()
                .ToLowerInvariant(), "[^a-z0-9,]+", "");

    private static string RemoveDiacritics(this string text)
    {
        var str = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var ch in str.Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
        {
            stringBuilder.Append(ch);
        }

        return stringBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC);
    }
}