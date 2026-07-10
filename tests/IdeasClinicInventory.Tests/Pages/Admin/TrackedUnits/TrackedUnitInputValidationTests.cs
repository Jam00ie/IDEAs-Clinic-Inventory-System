using IdeasClinicInventory.Web.Pages.Admin.TrackedUnits;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace IdeasClinicInventory.Tests.Pages.Admin.TrackedUnits;

public sealed class TrackedUnitInputValidationTests : IClassFixture<InventoryWebApplicationFactory>
{
    private readonly IModelMetadataProvider _metadataProvider;

    public TrackedUnitInputValidationTests(InventoryWebApplicationFactory factory)
    {
        _metadataProvider = factory.Services.GetRequiredService<IModelMetadataProvider>();
    }

    [Fact]
    public void Manual_identifier_is_not_implicitly_required_in_batch_mode()
    {
        // Conditional IValidatableObject rules decide which identifier field is
        // required. MVC must not add an unconditional rule to the hidden manual field.
        var metadata = _metadataProvider.GetMetadataForProperty(
            typeof(TrackedUnitInput),
            nameof(TrackedUnitInput.Identifier));

        Assert.False(metadata.IsRequired);
    }
}
