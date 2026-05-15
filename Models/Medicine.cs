namespace AbcPharmacy.Models;

/// <summary>
/// Represents a medicine in the ABC Pharmacy system.
/// </summary>
public class Medicine
{
    public int Id { get; set; }

    /// <summary>Full name of the medicine.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Optional notes / description.</summary>
    public string? Notes { get; set; }

    /// <summary>Expiry date of the medicine batch.</summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>Current quantity in stock.</summary>
    public int Quantity { get; set; }

    /// <summary>Selling price per unit (2 decimal places).</summary>
    public decimal Price { get; set; }

    /// <summary>Brand / manufacturer name.</summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>Timestamp when this record was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Timestamp when this record was last updated.</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
