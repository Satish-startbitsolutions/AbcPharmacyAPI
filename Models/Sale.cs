namespace AbcPharmacy.Models;

/// <summary>
/// Represents a single sale transaction for a medicine.
/// </summary>
public class Sale
{
    public int Id { get; set; }

    /// <summary>Foreign key to the medicine sold.</summary>
    public int MedicineId { get; set; }

    /// <summary>Navigation: medicine that was sold.</summary>
    public Medicine? Medicine { get; set; }

    /// <summary>Quantity sold in this transaction.</summary>
    public int QuantitySold { get; set; }

    /// <summary>Price per unit at time of sale (snapshot).</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Total amount = QuantitySold × UnitPrice.</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Optional customer / buyer name.</summary>
    public string? CustomerName { get; set; }

    /// <summary>Optional notes for the sale.</summary>
    public string? Notes { get; set; }

    /// <summary>Date and time the sale was recorded.</summary>
    public DateTime SoldAt { get; set; } = DateTime.UtcNow;
}
