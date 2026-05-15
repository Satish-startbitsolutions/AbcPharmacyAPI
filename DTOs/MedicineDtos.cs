using System.ComponentModel.DataAnnotations;

namespace AbcPharmacy.DTOs;

// ─────────────────────────────────────────────
// REQUEST DTOs
// ─────────────────────────────────────────────

/// <summary>Payload for creating a new medicine.</summary>
public class CreateMedicineRequest
{
    [Required(ErrorMessage = "Full name is required.")]
    [MaxLength(200, ErrorMessage = "Full name cannot exceed 200 characters.")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Expiry date is required.")]
    public DateTime ExpiryDate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Brand is required.")]
    [MaxLength(150, ErrorMessage = "Brand cannot exceed 150 characters.")]
    public string Brand { get; set; } = string.Empty;
}

/// <summary>Payload for updating an existing medicine (all fields optional).</summary>
public class UpdateMedicineRequest
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [Range(0, int.MaxValue)]
    public int? Quantity { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }

    [MaxLength(150)]
    public string? Brand { get; set; }
}

// ─────────────────────────────────────────────
// RESPONSE DTOs
// ─────────────────────────────────────────────

/// <summary>Full medicine details returned by the API.</summary>
public class MedicineResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Brand { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // ── Computed / UI helper fields ──────────────────────────────
    /// <summary>Days remaining before expiry (can be negative if already expired).</summary>
    public int DaysUntilExpiry { get; set; }

    /// <summary>True when expiry is within 30 days (red indicator).</summary>
    public bool IsExpiringSoon { get; set; }

    /// <summary>True when quantity is less than 10 (yellow indicator).</summary>
    public bool IsLowStock { get; set; }

    /// <summary>True when medicine is already expired.</summary>
    public bool IsExpired { get; set; }
}

/// <summary>Lightweight row for grid/list view (excludes Notes).</summary>
public class MedicineListItem
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Brand { get; set; } = string.Empty;

    // UI helper fields
    public int DaysUntilExpiry { get; set; }
    public bool IsExpiringSoon { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsExpired { get; set; }
}
