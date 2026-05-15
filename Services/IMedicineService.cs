using AbcPharmacy.DTOs;

namespace AbcPharmacy.Services;

public interface IMedicineService
{
    /// <summary>Returns paged + searchable list (grid view — no Notes).</summary>
    Task<PagedResult<MedicineListItem>> GetAllAsync(
        string? search,
        int page,
        int pageSize,
        string? sortBy,
        bool ascending,
        string? filter);

    /// <summary>Returns full medicine details including Notes.</summary>
    Task<MedicineResponse?> GetByIdAsync(int id);

    /// <summary>Creates a new medicine record.</summary>
    Task<MedicineResponse> CreateAsync(CreateMedicineRequest request);

    /// <summary>Updates an existing medicine (partial update).</summary>
    Task<MedicineResponse?> UpdateAsync(int id, UpdateMedicineRequest request);

    /// <summary>Deletes a medicine record.</summary>
    Task<bool> DeleteAsync(int id);
}
