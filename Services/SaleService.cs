using AbcPharmacy.DTOs;
using AbcPharmacy.Models;
using System.Globalization;

namespace AbcPharmacy.Services;

public class SaleService : ISaleService
{
    private readonly DataStore _store;

    public SaleService(DataStore store)
    {
        _store = store;
    }

    // ────────────────────────────────────────────────────────────
    // GET ALL
    // ────────────────────────────────────────────────────────────
    public Task<PagedResult<SaleResponse>> GetAllAsync(int? medicineId, int page, int pageSize, string? sortBy, bool ascending)
    {
        var query = _store.Sales.AsQueryable();

        if (medicineId.HasValue)
            query = query.Where(s => s.MedicineId == medicineId.Value);

        query = sortBy?.ToLower() switch
        {
            "id" => ascending
                ? query.OrderBy(s => s.Id)
                : query.OrderByDescending(s => s.Id),

            "medicinename" => ascending
                ? query.OrderBy(s => s.Medicine.FullName)
                : query.OrderByDescending(s => s.Medicine.FullName),

            "brand" => ascending
                ? query.OrderBy(s => s.Medicine.Brand)
                : query.OrderByDescending(s => s.Medicine.Brand),

            "customername" => ascending
                ? query.OrderBy(s => s.CustomerName)
                : query.OrderByDescending(s => s.CustomerName),

            "quantitysold" => ascending
                ? query.OrderBy(s => s.QuantitySold)
                : query.OrderByDescending(s => s.QuantitySold),

            "unitprice" => ascending
                ? query.OrderBy(s => s.UnitPrice)
                : query.OrderByDescending(s => s.UnitPrice),

            "totalamount" => ascending
                ? query.OrderBy(s => s.TotalAmount)
                : query.OrderByDescending(s => s.TotalAmount),

            "soldat" => ascending
                ? query.OrderBy(s => s.SoldAt)
                : query.OrderByDescending(s => s.SoldAt),

            _ => query.OrderByDescending(s => s.SoldAt)
        };
        var totalCount = query.Count();
        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => ToResponse(s))
            .ToList();

        return Task.FromResult(new PagedResult<SaleResponse>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize
        });
    }

    // ────────────────────────────────────────────────────────────
    // GET BY ID
    // ────────────────────────────────────────────────────────────
    public Task<SaleResponse?> GetByIdAsync(int id)
    {
        var sale = _store.Sales.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(sale == null ? null : ToResponse(sale));
    }

    // ────────────────────────────────────────────────────────────
    // CREATE SALE  (with stock validation & deduction)
    // ────────────────────────────────────────────────────────────
    public Task<(SaleResponse? Sale, string? Error)> CreateAsync(CreateSaleRequest request)
    {
        // ── Validate medicine exists ──────────────────────────────
        var medicine = _store.Medicines.FirstOrDefault(m => m.Id == request.MedicineId);
        if (medicine == null)
            return Task.FromResult<(SaleResponse?, string?)>((null, "Medicine not found."));

        // ── Validate not expired ──────────────────────────────────
        if (medicine.ExpiryDate.Date < DateTime.UtcNow.Date)
            return Task.FromResult<(SaleResponse?, string?)>((null,
                $"Cannot sell '{medicine.FullName}' — it has expired on {medicine.ExpiryDate:dd MMM yyyy}."));

        // ── Validate sufficient stock ─────────────────────────────
        if (medicine.Quantity < request.QuantitySold)
            return Task.FromResult<(SaleResponse?, string?)>((null,
                $"Insufficient stock. Available: {medicine.Quantity}, Requested: {request.QuantitySold}."));

        // ── Deduct stock ──────────────────────────────────────────
        medicine.Quantity  -= request.QuantitySold;
        medicine.UpdatedAt  = DateTime.UtcNow;

        // ── Create sale record ────────────────────────────────────
        var unitPrice = medicine.Price;
        var sale = new Sale
        {
            Id           = _store.NextSaleId(),
            MedicineId   = request.MedicineId,
            Medicine     = medicine,
            QuantitySold = request.QuantitySold,
            UnitPrice    = unitPrice,
            TotalAmount  = Math.Round(unitPrice * request.QuantitySold, 2),
            CustomerName = request.CustomerName?.Trim(),
            Notes        = request.Notes?.Trim(),
            SoldAt       = DateTime.UtcNow
        };

        _store.Sales.Add(sale);

        return Task.FromResult<(SaleResponse?, string?)>((ToResponse(sale), null));
    }

    // ────────────────────────────────────────────────────────────
    // SUMMARY
    // ────────────────────────────────────────────────────────────
    public Task<SalesSummary> GetSummaryAsync()
    {
        var latest = _store.Sales
            .OrderByDescending(s => s.SoldAt)
            .FirstOrDefault();

        var summary = new SalesSummary
        {
            TotalTransactions = _store.Sales.Count,
            TotalRevenue      = _store.Sales.Sum(s => s.TotalAmount),
            TotalUnitsSold    = _store.Sales.Sum(s => s.QuantitySold),
            LatestSale        = latest == null ? null : ToResponse(latest)
        };

        return Task.FromResult(summary);
    }

    // ────────────────────────────────────────────────────────────
    // MAPPER
    // ────────────────────────────────────────────────────────────
    private SaleResponse ToResponse(Sale s)
    {
        // Resolve medicine name from store if navigation not loaded
        var medicine = s.Medicine
            ?? _store.Medicines.FirstOrDefault(m => m.Id == s.MedicineId);

        return new SaleResponse
        {
            Id             = s.Id,
            MedicineId     = s.MedicineId,
            MedicineName   = medicine?.FullName   ?? "Unknown",
            MedicineBrand  = medicine?.Brand      ?? "Unknown",
            QuantitySold   = s.QuantitySold,
            UnitPrice      = s.UnitPrice,
            TotalAmount    = s.TotalAmount,
            CustomerName   = s.CustomerName,
            Notes          = s.Notes,
            SoldAt         = s.SoldAt
        };
    }
}
