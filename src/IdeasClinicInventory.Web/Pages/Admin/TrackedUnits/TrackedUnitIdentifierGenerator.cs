using System.Globalization;

namespace IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

/// <summary>
/// Produces deterministic identifiers for atomic tracked-unit batch creation.
/// </summary>
public static class TrackedUnitIdentifierGenerator
{
    public const int MaximumBatchSize = 500;
    public const int MaximumIdentifierLength = 100;

    public static IReadOnlyList<string> Generate(
        IdentifierGenerationMethod method,
        string? manualIdentifier,
        string? startingIdentifier,
        int quantity,
        string? prefix = null,
        string? postfix = null)
    {
        var baseIdentifiers = method switch
        {
            IdentifierGenerationMethod.Manual => GenerateManual(manualIdentifier),
            IdentifierGenerationMethod.NumericalOrder => GenerateNumerical(startingIdentifier, quantity),
            IdentifierGenerationMethod.AlphabeticalOrder => GenerateAlphabetical(startingIdentifier, quantity),
            _ => throw new ArgumentOutOfRangeException(nameof(method), "Choose a supported generation method.")
        };

        return ApplyAffixes(baseIdentifiers, prefix, postfix);
    }

    private static IReadOnlyList<string> GenerateManual(string? identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException("Enter an identifier.", nameof(identifier));
        }

        return [identifier.Trim()];
    }

    private static IReadOnlyList<string> GenerateNumerical(string? startingIdentifier, int quantity)
    {
        ValidateBatchQuantity(quantity);

        if (string.IsNullOrWhiteSpace(startingIdentifier) ||
            !startingIdentifier.All(char.IsAsciiDigit) ||
            !long.TryParse(startingIdentifier, NumberStyles.None, CultureInfo.InvariantCulture, out var start))
        {
            throw new ArgumentException("Enter a non-negative whole number, such as 1 or 25.", nameof(startingIdentifier));
        }

        var finalValue = checked(start + quantity - 1L);
        var width = startingIdentifier.Length > 1 && startingIdentifier[0] == '0'
            ? startingIdentifier.Length
            : 0;

        return Enumerable.Range(0, quantity)
            .Select(offset => (start + offset).ToString(width > 0 ? $"D{width}" : "D", CultureInfo.InvariantCulture))
            .ToList();
    }

    private static IReadOnlyList<string> GenerateAlphabetical(string? startingIdentifier, int quantity)
    {
        ValidateBatchQuantity(quantity);

        var normalized = startingIdentifier?.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(normalized) || !normalized.All(char.IsAsciiLetterUpper))
        {
            throw new ArgumentException("Enter letters only, such as A, C, or AA.", nameof(startingIdentifier));
        }

        var start = AlphabeticalIdentifierToNumber(normalized);
        _ = checked(start + quantity - 1L);

        return Enumerable.Range(0, quantity)
            .Select(offset => NumberToAlphabeticalIdentifier(start + offset))
            .ToList();
    }

    private static long AlphabeticalIdentifierToNumber(string identifier)
    {
        long value = 0;
        foreach (var character in identifier)
        {
            value = checked((value * 26) + (character - 'A' + 1));
        }

        return value;
    }

    private static string NumberToAlphabeticalIdentifier(long value)
    {
        var characters = new Stack<char>();
        while (value > 0)
        {
            value--;
            characters.Push((char)('A' + (value % 26)));
            value /= 26;
        }

        return new string(characters.ToArray());
    }

    private static void ValidateBatchQuantity(int quantity)
    {
        if (quantity is < 1 or > MaximumBatchSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                $"Create between 1 and {MaximumBatchSize} units in one batch.");
        }
    }

    private static IReadOnlyList<string> ApplyAffixes(
        IReadOnlyList<string> baseIdentifiers,
        string? prefix,
        string? postfix)
    {
        var normalizedPrefix = prefix?.Trim() ?? string.Empty;
        var normalizedPostfix = postfix?.Trim() ?? string.Empty;
        var identifiers = baseIdentifiers
            .Select(identifier => $"{normalizedPrefix}{identifier}{normalizedPostfix}")
            .ToList();

        if (identifiers.Any(identifier => identifier.Length > MaximumIdentifierLength))
        {
            throw new ArgumentException(
                $"Generated identifiers cannot exceed {MaximumIdentifierLength} characters.",
                nameof(prefix));
        }

        return identifiers;
    }
}
