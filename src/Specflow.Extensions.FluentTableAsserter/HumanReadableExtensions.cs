using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Specflow.Extensions.FluentTableAsserter.Properties.Exceptions;

namespace Specflow.Extensions.FluentTableAsserter;

internal static class HumanReadableExtensions
{
    public static bool EqualsHumanReadable(this string input1, string input2) =>
        input1.FromHumanReadableToNormalized() == input2.FromHumanReadableToNormalized();

    public static bool TryParseEnum(Type enumType, string humanReadableString, out object result)
    {
        var normalized = humanReadableString.FromHumanReadableToNormalized();
        var isEnumFlag = enumType.GetCustomAttribute(typeof(FlagsAttribute)) != null;

        if (!string.IsNullOrEmpty(normalized) || !isEnumFlag)
        {
            return Enum.TryParse(enumType, normalized, true, out result);
        }

        result = Activator.CreateInstance(enumType)
            ?? throw new InvalidOperationException($"Fail to instanciate {enumType}");

        return true;
    }

    public static object ParseEnum(Type enumType, string humanReadableString)
    {
        if (!TryParseEnum(enumType, humanReadableString, out var enumValue))
        {
            throw new CannotParseEnumToEnumValueException(humanReadableString, enumType);
        }

        return enumValue;
    }

    public static TEnum ParseEnum<TEnum>(string humanReadableString) where TEnum : Enum =>
        (TEnum)ParseEnum(typeof(TEnum), humanReadableString);

    public static string FromHumanReadableToNormalized(this string humanizedString) =>
        string.IsNullOrWhiteSpace(humanizedString)
            ? string.Empty
            : Regex.Replace(
                humanizedString.RemoveDiacritics().ToLowerInvariant(),
                "[^a-z0-9,]+",
                string.Empty
            );

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