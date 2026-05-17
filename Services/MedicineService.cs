using AbcPharmacy.DTOs;
using AbcPharmacy.Models;

namespace AbcPharmacy.Services;

public class MedicineService : IMedicineService
{
    private readonly DataStore _store;

    public MedicineService(DataStore store)
    {
        _store = store;
    }

    // ────────────────────────────────────────────────────────────
    // GET ALL  (list / grid — no Notes field)
    // ────────────────────────────────────────────────────────────
    //public Task<PagedResult<MedicineListItem>> GetAllAsync(
    //    string? search,
    //    int page,
    //    int pageSize,
    //    string? sortBy,
    //    bool ascending)
    public Task<PagedResult<MedicineListItem>> GetAllAsync(
    string? search,
    int page,
    int pageSize,
    string? sortBy,
    bool ascending,
    string? filter)
    {
        var query = _store.Medicines.AsQueryable();
        var totalCount = query.Count();
        var expiringSoonCount = query.Count(m =>
            m.ExpiryDate.Date >= DateTime.Today &&
            m.ExpiryDate.Date < DateTime.Today.AddDays(30));
        var lowStockCount = query.Count(m => m.Quantity < 10);
        var expiredCount = query.Count(m => m.ExpiryDate.Date < DateTime.Today);
        var today = DateTime.Today;
        var next30Days = today.AddDays(30);

        // 1. Stat card filter
        query = filter?.ToLower() switch
        {
            "expired" => query.Where(m =>
                m.ExpiryDate.Date < today),

            "lowstock" => query.Where(m =>
                m.Quantity < 10),

            "expiringsoon" => query.Where(m =>
                m.ExpiryDate.Date >= today &&
                m.ExpiryDate.Date <= next30Days),

            _ => query
        };

        // 2. Search inside selected filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();

            query = query.Where(m =>
                m.FullName.ToLower().Contains(term) ||
                m.Brand.ToLower().Contains(term));
        }

        // 3. Sort filtered/searched result
        query = (sortBy?.ToLower()) switch
        {
            "fullname" => ascending
                ? query.OrderBy(m => m.FullName)
                : query.OrderByDescending(m => m.FullName),

            "expiry" => ascending
                ? query.OrderBy(m => m.ExpiryDate)
                : query.OrderByDescending(m => m.ExpiryDate),

            "quantity" => ascending
                ? query.OrderBy(m => m.Quantity)
                : query.OrderByDescending(m => m.Quantity),

            "price" => ascending
                ? query.OrderBy(m => m.Price)
                : query.OrderByDescending(m => m.Price),

            "brand" => ascending
                ? query.OrderBy(m => m.Brand)
                : query.OrderByDescending(m => m.Brand),

            _ => query.OrderBy(m => m.FullName)
        };

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => ToListItem(m))
            .ToList();

        return Task.FromResult(new PagedResult<MedicineListItem>
        {
            Items = items,
            TotalCount = totalCount,
            ExpiringSoonCount = expiringSoonCount,
            LowStockCount = lowStockCount,
            ExpiredCount = expiredCount,
            Page = page,
            PageSize = pageSize
        });
    }

    // ────────────────────────────────────────────────────────────
    // GET BY ID  (full details including Notes)
    // ────────────────────────────────────────────────────────────
    public Task<MedicineResponse?> GetByIdAsync(int id)
    {
        var medicine = _store.Medicines.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(medicine == null ? null : ToResponse(medicine));
    }

    // ────────────────────────────────────────────────────────────
    // CREATE
    // ────────────────────────────────────────────────────────────
    public Task<MedicineResponse> CreateAsync(CreateMedicineRequest request)
    {
        var now = DateTime.UtcNow;
        var medicine = new Medicine
        {
            Id         = _store.NextMedicineId(),
            FullName   = request.FullName.Trim(),
            Notes      = request.Notes?.Trim(),
            ExpiryDate = request.ExpiryDate.ToUniversalTime(),
            Quantity   = request.Quantity,
            Price      = Math.Round(request.Price, 2),
            Brand      = request.Brand.Trim(),
            CreatedAt  = now,
            UpdatedAt  = now
        };

        _store.Medicines.Add(medicine);
        _store.SaveMedicines();
        return Task.FromResult(ToResponse(medicine));
    }

    // ────────────────────────────────────────────────────────────
    // UPDATE  (partial — only provided fields are updated)
    // ────────────────────────────────────────────────────────────
    public Task<MedicineResponse?> UpdateAsync(int id, UpdateMedicineRequest request)
    {
        var medicine = _store.Medicines.FirstOrDefault(m => m.Id == id);
        if (medicine == null) return Task.FromResult<MedicineResponse?>(null);

        if (request.FullName   != null) medicine.FullName   = request.FullName.Trim();
        if (request.Notes      != null) medicine.Notes      = request.Notes.Trim();
        if (request.ExpiryDate != null) medicine.ExpiryDate = request.ExpiryDate.Value.ToUniversalTime();
        if (request.Quantity   != null) medicine.Quantity   = request.Quantity.Value;
        if (request.Price      != null) medicine.Price      = Math.Round(request.Price.Value, 2);
        if (request.Brand      != null) medicine.Brand      = request.Brand.Trim();

        medicine.UpdatedAt = DateTime.UtcNow;
        _store.SaveMedicines();
        return Task.FromResult<MedicineResponse?>(ToResponse(medicine));
    }

    // ────────────────────────────────────────────────────────────
    // DELETE
    // ────────────────────────────────────────────────────────────
    public Task<bool> DeleteAsync(int id)
    {
        var medicine = _store.Medicines.FirstOrDefault(m => m.Id == id);
        if (medicine == null) return Task.FromResult(false);

        _store.Medicines.Remove(medicine);
        _store.SaveMedicines();
        return Task.FromResult(true);
    }

    // ────────────────────────────────────────────────────────────
    // PRIVATE MAPPERS
    // ────────────────────────────────────────────────────────────

    private static MedicineResponse ToResponse(Medicine m)
    {
        var days = (int)(m.ExpiryDate.Date - DateTime.UtcNow.Date).TotalDays;
        return new MedicineResponse
        {
            Id              = m.Id,
            FullName        = m.FullName,
            Notes           = m.Notes,
            ExpiryDate      = m.ExpiryDate,
            Quantity        = m.Quantity,
            Price           = m.Price,
            Brand           = m.Brand,
            CreatedAt       = m.CreatedAt,
            UpdatedAt       = m.UpdatedAt,
            DaysUntilExpiry = days,
            IsExpired       = days < 0,
            IsExpiringSoon  = days >= 0 && days < 30,
            IsLowStock      = m.Quantity < 10
        };
    }

    private static MedicineListItem ToListItem(Medicine m)
    {
        var days = (int)(m.ExpiryDate.Date - DateTime.UtcNow.Date).TotalDays;
        return new MedicineListItem
        {
            Id              = m.Id,
            FullName        = m.FullName,
            ExpiryDate      = m.ExpiryDate,
            Quantity        = m.Quantity,
            Price           = m.Price,
            Brand           = m.Brand,
            DaysUntilExpiry = days,
            IsExpired       = days < 0,
            IsExpiringSoon  = days >= 0 && days < 30,
            IsLowStock      = m.Quantity < 10
        };
    }
}
