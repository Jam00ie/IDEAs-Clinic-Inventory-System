using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdeasClinicInventory.Web.Models;

/// <summary>
/// Identifies a physical place where inventory can be stored.
/// </summary>
public sealed class InventoryLocation
{
    public int Id { get; set; }

    [Display(Name = "Room code")]
    [Required]
    [StringLength(20)]
    public string RoomCode { get; set; } = "0";

    [Display(Name = "Storage unit code")]
    [Required]
    [StringLength(20)]
    [RegularExpression("^(0|[A-Za-z][A-Za-z0-9]*)$",
        ErrorMessage = "Use 0 or a code that starts with a letter and contains only letters and numbers.")]
    public string StorageUnitCode { get; set; } = "0";

    [Display(Name = "Storage unit level")]
    [Range(0, int.MaxValue)]
    public int StorageUnitLevel { get; set; }

    [StringLength(50)]
    public string? Subunit { get; set; }

    [Display(Name = "Reference image path")]
    [StringLength(2_048)]
    public string? ReferenceImagePath { get; set; }

    [StringLength(4_000)]
    public string? Notes { get; set; }

    [Display(Name = "Created (UTC)")]
    public DateTimeOffset CreatedAtUtc { get; set; }

    [Display(Name = "Updated (UTC)")]
    public DateTimeOffset UpdatedAtUtc { get; set; }

    /// <summary>
    /// Builds the human-readable code from structured location fields.
    /// </summary>
    /// <remarks>
    /// The code is derived instead of stored, preventing it from becoming inconsistent
    /// when an administrator edits the room, level, storage unit, or subunit.
    /// </remarks>
    [NotMapped]
    public string LocationCode
    {
        get
        {
            var roomCode = NormalizeCode(RoomCode, "0");
            var storageUnitCode = NormalizeCode(StorageUnitCode, "0");

            if (roomCode == "0")
            {
                return "0-00";
            }

            if (StorageUnitLevel == 0 || storageUnitCode == "0")
            {
                return $"{roomCode}-00";
            }

            var baseCode = $"{roomCode}-{StorageUnitLevel}{storageUnitCode}";
            var subunit = NormalizeCode(Subunit, string.Empty);
            return string.IsNullOrEmpty(subunit) ? baseCode : $"{baseCode}-{subunit}";
        }
    }

    /// <summary>
    /// SQL Server changes this value on every update so simultaneous edits can be detected.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    public ICollection<TrackedUnit> TrackedUnits { get; set; } = [];

    internal void NormalizeCodes()
    {
        // Normalizing before persistence makes the composite unique index reliable even
        // when users enter equivalent codes with different casing or surrounding spaces.
        RoomCode = NormalizeCode(RoomCode, "0");
        StorageUnitCode = NormalizeCode(StorageUnitCode, "0");
        Subunit = NormalizeCode(Subunit, string.Empty) is { Length: > 0 } normalizedSubunit
            ? normalizedSubunit
            : null;

        // Every partially designated location displays as ROOM-00. Store those
        // fields identically as well, so the unique index cannot accept aliases.
        if (RoomCode == "0")
        {
            StorageUnitLevel = 0;
            StorageUnitCode = "0";
            Subunit = null;
        }
        else if (StorageUnitLevel == 0 || StorageUnitCode == "0")
        {
            StorageUnitLevel = 0;
            StorageUnitCode = "0";
            Subunit = null;
        }
    }

    private static string NormalizeCode(string? value, string fallback)
    {
        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : value.Trim().ToUpperInvariant();
    }
}
