using IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;

namespace IdeasClinicInventory.Tests.Pages.Admin.TrackedUnits;

public sealed class TrackedUnitIdentifierGeneratorTests
{
    [Fact]
    public void Numerical_batch_starts_at_requested_index()
    {
        var identifiers = TrackedUnitIdentifierGenerator.Generate(
            IdentifierGenerationMethod.NumericalOrder,
            null,
            "2",
            5);

        Assert.Equal(["2", "3", "4", "5", "6"], identifiers);
    }

    [Fact]
    public void Alphabetical_batch_starts_at_requested_index()
    {
        var identifiers = TrackedUnitIdentifierGenerator.Generate(
            IdentifierGenerationMethod.AlphabeticalOrder,
            null,
            "C",
            5);

        Assert.Equal(["C", "D", "E", "F", "G"], identifiers);
    }

    [Fact]
    public void Alphabetical_batch_continues_after_z()
    {
        var identifiers = TrackedUnitIdentifierGenerator.Generate(
            IdentifierGenerationMethod.AlphabeticalOrder,
            null,
            "Y",
            5);

        Assert.Equal(["Y", "Z", "AA", "AB", "AC"], identifiers);
    }

    [Fact]
    public void Numerical_batch_preserves_explicit_zero_padding()
    {
        var identifiers = TrackedUnitIdentifierGenerator.Generate(
            IdentifierGenerationMethod.NumericalOrder,
            null,
            "007",
            3);

        Assert.Equal(["007", "008", "009"], identifiers);
    }

    [Fact]
    public void Manual_identifier_is_trimmed_and_creates_one_unit()
    {
        var identifiers = TrackedUnitIdentifierGenerator.Generate(
            IdentifierGenerationMethod.Manual,
            " Kit-03 ",
            null,
            25);

        Assert.Equal(["Kit-03"], identifiers);
    }

    [Fact]
    public void Numerical_batch_applies_prefix_and_postfix()
    {
        var identifiers = TrackedUnitIdentifierGenerator.Generate(
            IdentifierGenerationMethod.NumericalOrder,
            null,
            "2",
            3,
            "KC-",
            "-UW");

        Assert.Equal(["KC-2-UW", "KC-3-UW", "KC-4-UW"], identifiers);
    }

    [Fact]
    public void Alphabetical_batch_applies_prefix_and_postfix()
    {
        var identifiers = TrackedUnitIdentifierGenerator.Generate(
            IdentifierGenerationMethod.AlphabeticalOrder,
            null,
            "C",
            3,
            "IDEAS-",
            "-KIT");

        Assert.Equal(["IDEAS-C-KIT", "IDEAS-D-KIT", "IDEAS-E-KIT"], identifiers);
    }
}
