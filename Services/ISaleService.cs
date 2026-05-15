using AbcPharmacy.DTOs;

namespace AbcPharmacy.Services;

public interface ISaleService
{
    /// <summary>Returns paged list of all sale records.</summary>
    Task<PagedResult<SaleResponse>> GetAllAsync(int? medicineId, int page, int pageSize, string? sortBy, bool ascending);

    /// <summary>Returns a single sale record by ID.</summary>
    Task<SaleResponse?> GetByIdAsync(int id);

    /// <summary>Records a new sale and deducts stock from medicine.</summary>
    Task<(SaleResponse? Sale, string? Error)> CreateAsync(CreateSaleRequest request);

    /// <summary>Returns aggregated sales statistics.</summary>
    Task<SalesSummary> GetSummaryAsync();
}
