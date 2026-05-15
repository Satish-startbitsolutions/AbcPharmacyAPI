using System.ComponentModel.DataAnnotations;

namespace AbcPharmacy.DTOs;

// ─────────────────────────────────────────────
// REQUEST DTOs
// ─────────────────────────────────────────────

/// <summary>Payload for recording a new sale.</summary>
public class CreateSaleRequest
{
    [Required(ErrorMessage = "MedicineId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Invalid MedicineId.")]
    public int MedicineId { get; set; }

    [Required(ErrorMessage = "Quantity sold is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity sold must be at least 1.")]
    public int QuantitySold { get; set; }

    [MaxLength(150)]
    public string? CustomerName { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

// ─────────────────────────────────────────────
// RESPONSE DTOs
// ─────────────────────────────────────────────

/// <summary>Full sale record returned by the API.</summary>
public class SaleResponse
{
    public int Id { get; set; }
    public int MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string MedicineBrand { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? Notes { get; set; }
    public DateTime SoldAt { get; set; }
}

/// <summary>Summary statistics for the sales dashboard.</summary>
public class SalesSummary
{
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalUnitsSold { get; set; }
    public SaleResponse? LatestSale { get; set; }
}
